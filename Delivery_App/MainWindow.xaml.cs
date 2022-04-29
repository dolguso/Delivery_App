using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.IO;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Windows.Threading;
using Delivery_App.Model;
using System.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using ToastNotifications.Core;
using System.Windows.Controls;
using System.Windows.Data;

namespace Delivery_App
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class MainWindow : Window
    {

        //보안을 위해 관련 정보는 모두 삭제하여 실행은 불가//
        private NetworkStream stream;
        private BackgroundWorker bgw_socket = new BackgroundWorker();
        private JObject loginData;
        string jwt_value = "";
        public String m_biz_acc_cd = "";
        public String m_strshop_id = "";
        public String m_version = "";
        public String m_PCNAME = System.Environment.MachineName;
        private List<string> orderNos = new List<string>();
        private List<Class.order> order_list = new List<Class.order>();
        private List<Class.order> new_order_list_print = new List<Class.order>();
        private List<Class.order> new_order_list = new List<Class.order>();
        static public List<string> printed_list = new List<string>();
        private string web_url = "";
        private string socket_ip_address = "";
        private int socket_port = 0;
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer alarm = new DispatcherTimer();
        private ViewModel myModel = new ViewModel();
        private SoundPlayer order_sound_play = new System.Media.SoundPlayer(Directory.GetCurrentDirectory() + "/alarm_sound/Delivery_App/Delivery_App_order.wav");
        private SoundPlayer cancel_sound_play = new System.Media.SoundPlayer(Directory.GetCurrentDirectory() + "/alarm_sound/Delivery_App/Delivery_App_cancel.wav");
        public WindowState pre_state;
        private int today_count = 0;
        Window noti;
        Grid noti_grid;
        OrderCardControl card_alarm_01;
        OrderCardControl card_alarm_02;
        RoutedEventArgs newEventArgs = new RoutedEventArgs(OrderCardControl.CustomTestEvent);

        private Window show_noti_window()
        {
            Window noti = new Window();
            noti.WindowState = WindowState.Maximized;
            noti.WindowStyle = WindowStyle.None;
            noti.Background = Brushes.Transparent;
            noti.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            noti.AllowsTransparency = true;
            noti.ShowInTaskbar = false;
            noti.Show();
            return noti;
        }

        private OrderCardControl init_order_card(Grid input_parent, int row_property)
        {
            OrderCardControl card = new OrderCardControl(myModel, alarm, this, noti_grid);
            card.VerticalAlignment = VerticalAlignment.Bottom;
            card.HorizontalAlignment = HorizontalAlignment.Right;
            card.Visibility = Visibility.Visible;
            card.SetBinding(UserControl.VisibilityProperty, new Binding()
            {
                Path = new PropertyPath("Card_visible"),
                Source = myModel
            });
            card.SetValue(Grid.RowProperty, row_property);
            input_parent.Children.Add(card);
            return card;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = myModel;
            loginData = sendLogin();
            m_biz_acc_cd = loginData["data"]["biz_acc_cd"].ToString();
            jwt_value = loginData["data"]["auth_token"].ToString();
            myModel.SetJwt(jwt_value);
            noti = show_noti_window();
            noti_grid = new Grid();
            noti_grid.VerticalAlignment = VerticalAlignment.Stretch;
            noti_grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            noti_grid.Visibility = Visibility.Hidden;
            noti.Content = noti_grid;
            card_alarm_01 = init_order_card(big_grid, 1);
            card_alarm_02 = init_order_card(noti_grid, 0);
            myModel.InitNotifier(noti, this);
            Delivery_App.Notify.setNotify(this, noti_grid);
            get_store_status();
            RawPrinterHelper.SendStringToPrinter(Program.printerName, "실행 완료");
            RawPrinterHelper.PartialCut();
            bgw_socket.DoWork += Bgw_socket_DoWork;
            bgw_socket.RunWorkerAsync();
            web_view.LoadUrl(web_url);

            List<Delivery_App.Class.order> result = get_new_orders();
            if (result.Count > 0)
            {
                System.Threading.Thread.Sleep(500);
                for (int i = 0; i < result.Count(); i++)
                    get_order_detail(result[i].order_cd.ToString());

                alarm.Start();

                this.Dispatcher.BeginInvoke(new System.Threading.ThreadStart(() =>
                {
                    if (this.ShowInTaskbar == false)
                        myModel.Notifier.ShowInformation("새로운 주문이 들어왔습니다!", myModel.Options);

                    card_alarm_01.RaiseEvent(newEventArgs); // This change fixes the issue.
                    card_alarm_02.RaiseEvent(newEventArgs); // This change fixes the issue.
                }));
                order_sound_play.Play();
                PrinterFunction.print_receipt(new_order_list_print);
            }

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            alarm.Tick += Alarm_Tick;
            alarm.Interval = new TimeSpan(0, 0, 0, 0, 500);

        }

        private void Alarm_Tick(object sender, EventArgs e)
        {
            myModel.Flash();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            myModel.Update();
        }


        private void get_store_status()
        {
            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myModel.baseUrl);
            request.Method = "GET";
            request.Timeout = 30 * 1000; // 30초
            request.Accept = "application/json";
            request.Headers.Add("Authorization", myModel.Jwt);
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;
                Console.WriteLine(status);  // 정상이면 "OK"

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            JObject temp = JObject.Parse(responseText);
            List<Delivery_App.Class.store> temp_store_list = new List<Class.store>();
            temp_store_list.Add(new Delivery_App.Class.store
            {
                //shop_cd = temp["data"][i]["shop_cd"].ToString(),
                istmpstop = Int32.Parse(temp["data"]["istmpstop"].ToString()),
                isstop = Int32.Parse(temp["data"]["isstop"].ToString()),
            });
            myModel.SetStoreList(temp_store_list);
        }

        private JObject get_order_detail(string order_cd)
        {
            string responseText = string.Empty;
            String postData = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myModel.baseUrl);
            request.Method = "GET";
            request.Timeout = 30 * 1000; // 30초
            request.Accept = "application/json";
            request.Headers.Add("Authorization", myModel.Jwt);
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;
                Console.WriteLine(status);  // 정상이면 "OK"

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            JObject temp = JObject.Parse(responseText);

            today_count++;

            Delivery_App.Class.order new_order = new Delivery_App.Class.order
            {
                today_count = today_count,
                order_cd = temp["data"]["order_cd"].ToString(),
                shop_name = temp["data"]["shop_name"].ToString(),
                order_disposable = temp["data"]["order_disposable"].ToString(),
                order_dtime = temp["data"]["order_dtime"].ToString(),
                //level_gubun = temp["data"]["level_gubun"].ToString(),
                order_delivgb = temp["data"]["order_delivgb"].ToString(),
                pay_method = temp["data"]["pay_method"].ToString(),
                order_menus = ((JArray)temp["data"]["order_menus"]).Select(y => new Delivery_App.Class.menu
                {
                    menu_price_cd = y["menu_price_cd"].ToString(),
                    menu_price_amount = Int32.Parse(y["menu_price_amount"].ToString()),
                    count = Int32.Parse(y["count"].ToString()),
                    option_group_list = ((JArray)y["option_list"]).Select(z => new Delivery_App.Class.option_group
                    {
                        option_group_name = z["option_group_name"].ToString(),
                        option_group_cd = z["option_group_cd"].ToString(),
                        options = ((JArray)z["options"]).Select(a => new Delivery_App.Class.option
                        {
                            option_cd = a["option_cd"].ToString(),
                            option_name = a["option_name"].ToString(),
                            option_amount = Int32.Parse(a["option_amount"].ToString()),
                        }).ToList(),
                    }).ToList(),
                    menu_price_name = y["menu_price_name"].ToString(),
                    menu_cd = y["menu_cd"].ToString(),
                    menu_name = y["menu_name"].ToString(),
                }).ToList(),
                order_tip_amount = Int32.Parse(temp["data"]["order_tip_amount"].ToString()),
                order_amount = Int32.Parse(temp["data"]["order_amount"].ToString()),
                order_status = temp["data"]["order_status"].ToString(),
                shop_origin = temp["data"]["shop_origin"].ToString()
            };
            Console.WriteLine(new_order.order_amount);
            Console.WriteLine(new_order.order_menus[0].menu_name);

            myModel.NewOrder(new_order);

            if (!new_order_list_print.Any(order => order.order_cd == order_cd) && !printed_list.Contains(order_cd))
                new_order_list_print.Add(new_order); ;
            return temp;
        }

        private List<Delivery_App.Class.order> get_new_orders(int count = 0)
        {
            string responseText = string.Empty;
            String postData = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myModel.baseUrl);
            request.Method = "GET";
            request.Timeout = 30 * 1000; // 30초
            request.Accept = "application/json";
            request.Headers.Add("Authorization", myModel.Jwt);

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;
                Console.WriteLine(status);  // 정상이면 "OK"

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            JObject temp = JObject.Parse(responseText);
            int data_count = temp["data"].Count();
            order_list = temp["data"].Select(x => new Delivery_App.Class.order
            {
                order_cd = x["order_cd"].ToString(),
                //shop_name = x["shop_name"].ToString(),
                //order_disposable = x["order_disposable"].ToString(),
                order_dtime = x["order_dtime"].ToString(),
                level_gubun = x["level_gubun"].ToString(),
                order_delivgb = x["order_delivgb"].ToString(),
                order_menus = ((JArray)x["order_menus"]).Select(y => new Delivery_App.Class.menu
                {
                    menu_price_cd = y["menu_price_cd"].ToString(),
                    menu_price_amount = Int32.Parse(y["menu_price_amount"].ToString()),
                    count = Int32.Parse(y["count"].ToString()),
                    option_group_list = ((JArray)y["option_list"]).Select(z => new Delivery_App.Class.option_group
                    {
                        option_group_name = z["option_group_name"].ToString(),
                        option_group_cd = z["option_group_cd"].ToString(),
                        options = ((JArray)z["options"]).Select(a => new Delivery_App.Class.option
                        {
                            option_cd = a["option_cd"].ToString(),
                            option_name = a["option_name"].ToString(),
                            option_amount = Int32.Parse(a["option_amount"].ToString()),
                        }).ToList(),
                    }).ToList(),
                    menu_price_name = y["menu_price_name"].ToString(),
                    menu_cd = y["menu_cd"].ToString(),
                    menu_name = y["menu_name"].ToString(),
                }).ToList(),
            }).ToList();

            List<Delivery_App.Class.order> result_order_list = new List<Class.order>();

            int pre_count = orderNos.Count;

            for (int i = 0; i < data_count; i++)
            {
                try
                {
                    string orderNo = order_list.ElementAt(i).order_cd;
                    if (!orderNos.Contains(orderNo))
                    {
                        result_order_list.Add(order_list[i]);
                        orderNos.Add(orderNo);
                    }
                }
                catch
                {
                    Console.WriteLine("No data");
                }
            }
            int now_count = orderNos.Count;
            if (now_count == pre_count && count < 5)
            {
                Console.WriteLine("No diff");
                result_order_list = get_new_orders(count += 1);
            }
            Console.WriteLine("Diff!");
            return result_order_list;
        }

        private void Bgw_socket_DoWork(object sender, DoWorkEventArgs e)
        {
            IPEndPoint clientAddress = new IPEndPoint(IPAddress.Any, 0);
            TcpClient client = new TcpClient(clientAddress);

            IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(socket_ip_address), socket_port);
            client.Connect(serverAddress);

            stream = client.GetStream();

            int length = 0;
            string data = null;
            byte[] bytes = new byte[256];

            if (stream != null)
            {
                String loginMsg = "";

                stream.Write(Encoding.UTF8.GetBytes(loginMsg), 0, loginMsg.Length);
            }

            Timer timer = new System.Timers.Timer();
            timer.Interval = 10 * 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
            while (true)
            {
                if ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.Default.GetString(bytes, 0, length);
                    if (data != "")
                    {
                        JObject temp;
                        if (data.Contains("neworder"))
                        {
                            System.Threading.Thread.Sleep(1000);
                            List<Delivery_App.Class.order> result = get_new_orders();
                            System.Threading.Thread.Sleep(1000);
                            for (int i = 0; i < result.Count(); i++)
                                get_order_detail(result[i].order_cd.ToString());
                            alarm.Start();

                            order_sound_play.Play();
                            card_alarm_01.RaiseEvent(newEventArgs); // This change fixes the issue.
                            card_alarm_02.RaiseEvent(newEventArgs); // This change fixes the issue.

                            PrinterFunction.print_receipt(new_order_list_print);
                        }
                        else if (data.Contains("newcancel"))
                        {
                            temp = JObject.Parse(data);
                            if (orderNos.Contains(temp["orderno"].ToString()))
                                orderNos.Remove(temp["orderno"].ToString());

                            this.Dispatcher.BeginInvoke(new System.Threading.ThreadStart(() =>
                            {
                                myModel.CancelOrder(temp["orderno"].ToString(), alarm);
                            }));
                            cancel_sound_play.Play();
                            if (orderNos.Count == 0)
                            {
                                alarm.Stop();
                                this.Dispatcher.BeginInvoke(new System.Threading.ThreadStart(() =>
                                {
                                    myModel.ResetFlash();
                                }));
                            }

                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            stream.Write(Encoding.Default.GetBytes("alive"), 0, "alive".Length);
        }

        JObject sendLogin()
        {
            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myModel.baseUrl);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Method = "POST";
            request.Timeout = 30 * 1000; // 30초
            String postData = "";
            byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);
            request.ContentLength = sendData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;
                Console.WriteLine(status);  // 정상이면 "OK"

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            return JObject.Parse(responseText);
        }

        private void btn_setting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myModel.SettingOnOff();
        }
        private void btn_hide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Hide();
            noti_grid.Visibility = Visibility.Visible;
        }
        private void btn_close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            System.Windows.Application.Current.Shutdown();
        }
        private void btn_windowstate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ShowInTaskbar = true;
            noti_grid.Visibility = Visibility.Hidden;
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                btn_maximize.Visibility = Visibility.Visible;
                btn_minimize.Visibility = Visibility.Hidden;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                btn_minimize.Visibility = Visibility.Visible;
                btn_maximize.Visibility = Visibility.Hidden;
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myModel.SetStoreStatusOnOff();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                btn_minimize.Visibility = Visibility.Visible;
                btn_maximize.Visibility = Visibility.Hidden;
            }
            else
            {
                btn_minimize.Visibility = Visibility.Hidden;
                btn_maximize.Visibility = Visibility.Visible;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btn_hide.RaiseEvent(new RoutedEventArgs(Border.MouseLeftButtonDownEvent));
        }
    }
}
