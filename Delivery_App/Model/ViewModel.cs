using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using ToastNotifications.Core;

namespace Delivery_App.Model
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _jwt;
        private DateTime _now;
        private string _now_time;
        private string _now_date;
        private Brush _alert;
        private Visibility _setting_visible;
        private Visibility _web_visible;
        private Brush _setting_color;
        private Brush _setting_bg_color;
        private List<Delivery_App.Class.store> _store_list;
        private List<Delivery_App.Class.order> _order_list;
        private Delivery_App.Class.order _now_order;
        private Brush _store_working;
        private string _store_working_status;
        private string _order_type;
        private Visibility _card_visible;
        private int _prepare_time = 20;
        public string baseUrl = "";//baseURL
        private Notifier _notifier;
        private MessageOptions _options;
        private int _today_count;
        private string _order_summary;
        private string _order_menus;
        private string _order_count;
        private Visibility _order_count_visible;

        public ViewModel()
        {
            _now = DateTime.Now;
            _jwt = "";
            _now_time = _now.ToString("HH:mm");
            _now_date = _now.ToString("MM.dd(ddd)");
            _alert = new SolidColorBrush(Color.FromRgb(48, 49, 64));
            _setting_visible = Visibility.Hidden;
            _web_visible = Visibility.Visible;
            _setting_color = Brushes.White;
            _setting_bg_color = Brushes.Transparent;
            _store_list = new List<Class.store>();
            _order_list = new List<Class.order>();
            _now_order = new Class.order();
            _store_working = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            _store_working_status = "영업중";
            _order_type = "";
            _card_visible = Visibility.Hidden;
            _prepare_time = 20;
            _order_count_visible = Visibility.Hidden;
        }

        public Visibility Order_count_visible
        {
            get { return _order_count_visible; }
            set
            {
                _order_count_visible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Order_count_visible"));
                }
            }
        }

        public string Order_count
        {
            get { return _order_count; }
            set
            {
                _order_count = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Order_count"));
                }
            }
        }

        public string Order_summary
        {
            get
            {
                return _order_summary;
            }
            set
            {
                _order_summary = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Order_summary"));
                }
            }
        }

        public string Order_menus
        {
            get
            {
                return _order_menus;
            }
            set
            {
                _order_menus = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Order_menus"));
                }
            }
        }

        public Notifier Notifier
        {
            get { return _notifier; }
            set
            {
                _notifier = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Notifier"));
                }
            }
        }

        public MessageOptions Options
        {
            get { return _options; }
            set
            {
                _options = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Options"));
                }
            }
        }

        public void InitNotifier(Window input_noti, Window input_window)
        {
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: input_noti,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 20);

                cfg.LifetimeSupervisor = new CountBasedLifetimeSupervisor(maximumNotificationCount: MaximumNotificationCount.UnlimitedNotifications());

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 350;
            });
            _options = new MessageOptions
            {
                FontSize = 22, // set notification font size
                ShowCloseButton = false, // set the option to show or hide notification close button
                FreezeOnMouseEnter = true, // set the option to prevent notification dissapear automatically if user move cursor on it
                NotificationClickAction = n => // set the callback for notification click event
                {
                    //this.WindowState = pre_state;
                    input_window.ShowInTaskbar = true;
                    input_window.Show();
                    n.Close(); // call Close method to remove notification
                }
            };
        }

        public string Jwt
        {
            get { return _jwt; }
            private set
            {
                _jwt = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Jwt"));
                }
            }
        }

        public Visibility Setting_visible
        {
            get { return _setting_visible; }
            private set
            {
                _setting_visible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Setting_visible"));
                }
            }
        }

        public Visibility Web_visible
        {
            get { return _web_visible; }
            private set
            {
                _web_visible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Web_visible"));
                }
            }
        }

        public Brush Alert
        {
            get { return _alert; }
            private set
            {
                _alert = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Alert"));
                }
            }
        }

        public DateTime Now
        {
            get { return _now; }
            private set
            {
                _now = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Now"));
                }
            }
        }
        public string Now_time
        {
            get { return _now_time; }
            private set
            {
                _now_time = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Now_time"));
                }
            }
        }
        public string Now_date
        {
            get { return _now_date; }
            private set
            {
                _now_date = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Now_date"));
                }
            }
        }

        public Brush Setting_color
        {
            get { return _setting_color; }
            private set
            {
                _setting_color = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Setting_color"));
                }
            }
        }
        public Brush Setting_bg_color
        {
            get { return _setting_bg_color; }
            private set
            {
                _setting_bg_color = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Setting_bg_color"));
                }
            }
        }

        public List<Delivery_App.Class.store> Store_list
        {
            get { return _store_list; }
            private set
            {
                _store_list = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Store_list"));
                }
            }
        }

        public Brush Store_working
        {
            get { return _store_working; }
            private set
            {
                _store_working = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Store_working"));
                }
            }
        }
        public string Store_working_status
        {
            get { return _store_working_status; }
            private set
            {
                _store_working_status = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Store_working_status"));
                }
            }
        }

        public Delivery_App.Class.order Now_order
        {
            get { return _now_order; }
            private set
            {
                _now_order = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Now_order"));
                }
            }
        }
        public List<Delivery_App.Class.order> Order_list
        {
            get { return _order_list; }
            private set
            {
                _order_list = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Order_list"));
                }
            }
        }


        public int Today_count
        {
            get { return _today_count; }
            private set
            {
                _today_count = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Today_count"));
                }
            }
        }

        public string Order_type
        {
            get { return _order_type; }
            private set
            {
                _order_type = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Order_type"));
                }
            }
        }

        public Visibility Card_visible
        {
            get { return _card_visible; }
            private set
            {
                _card_visible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Card_visible"));
                }
            }
        }

        public int Prepare_time
        {
            get { return _prepare_time; }
            private set
            {
                _prepare_time = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Prepare_time"));
                }
            }
        }

        public void SetJwt(string input_jwt)
        {
            Jwt = input_jwt;
        }

        public void Update()
        {
            Now = DateTime.Now;
            if (Now_date != Now.ToString("MM.dd(ddd)"))
                Now_date = Now.ToString("MM.dd(ddd)");
            Now_time = Now.ToString("HH:mm");
        }

        public void Flash()
        {
            if (Setting_visible == Visibility.Hidden)
                Setting_bg_color = Brushes.Transparent;
            Alert = Alert.ToString() == "#FF303140" ? new SolidColorBrush(Color.FromRgb(0, 255, 255)) : new SolidColorBrush(Color.FromRgb(48, 49, 64));
        }

        public void ResetFlash()
        {
            Alert = new SolidColorBrush(Color.FromRgb(48, 49, 64));
        }

        public void SettingOnOff()
        {
            Setting_color = Setting_visible == Visibility.Hidden ? new SolidColorBrush(Color.FromRgb(48, 49, 64)) : Brushes.White;
            Setting_bg_color = Setting_visible == Visibility.Hidden ? Brushes.White : new SolidColorBrush(Color.FromRgb(48, 49, 64));
            Web_visible = Setting_visible == Visibility.Hidden ? Visibility.Hidden : Visibility.Visible;
            Setting_visible = Setting_visible == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }

        public void SetStoreList(List<Delivery_App.Class.store> input_store_list)
        {
            Store_list = input_store_list;
            SetStoreWorkingValue(input_store_list[0]);
        }

        public void SetStoreWorkingValue(Delivery_App.Class.store input_store)
        {
            Store_working = input_store.istmpstop == 1 ? new SolidColorBrush(Color.FromRgb(255, 255, 0)) : input_store.isstop == 1 ? new SolidColorBrush(Color.FromRgb(255, 0, 0)) : new SolidColorBrush(Color.FromRgb(0, 255, 43));
            Store_working_status = input_store.istmpstop == 1 ? "임시영업중지" : input_store.isstop == 1 ? "영업중지" : "영업중";
        }

        public void SetStoreStatusOnOff()
        {
            var temp = Store_list;
            temp[0].isstop = Math.Abs(temp[0].isstop - 1);
            Store_list = temp;
            SetStoreWorkingValue(temp[0]);
        }

        public void NewOrder(Delivery_App.Class.order new_order)
        {
            var temp = Order_list;
            temp.Add(new_order);

            var value = temp.Count == 1 ? new_order : temp[0];
            Now_order = value;
            Order_summary = String.Format("메뉴 {0}개 · 총 {1}원 · {2}", value.order_menus == null ? 0 : value.order_menus.Count, value.order_amount, value.pay_method);
            string result = "";
            if (value.order_menus != null)
                for (int i = 0; i < value.order_menus.Count; i++)
                {
                    result += value.order_menus[i].menu_name;
                    string side = "(";
                    for (int j = 0; j < value.order_menus[i].option_group_list.Count; j++)
                    {
                        for (int k = 0; k < value.order_menus[i].option_group_list[j].options.Count; k++)
                        {
                            side += value.order_menus[i].option_group_list[j].options[k].option_name + "추가";
                            if (j != value.order_menus[i].option_group_list.Count - 1 || k != value.order_menus[i].option_group_list[j].options.Count() - 1)
                                side += "/";
                            else
                                side += ")";
                        }
                    }
                    result += side;
                    result += " " + value.order_menus[i].count + "개";
                    if (i != value.order_menus.Count - 1)
                        result += ", ";
                }
            Order_menus = result;
            Order_type = value.order_delivgb;
            Card_visible = Visibility.Visible;
            Order_list = temp;
            Today_count = value.today_count;
            Order_count = Order_list.Count > 99 ? "99+" : Order_list.Count.ToString();
            Order_count_visible = Order_list.Count == 0 ? Visibility.Hidden : Visibility.Visible;
        }

        public void RemoveOrder(DispatcherTimer alarm_timer)
        {
            var temp = Order_list;
            if (temp.Count > 0)
                temp.RemoveAt(0);
            if (temp.Count > 0)
            {
                Now_order = temp[0];
                var value = temp[0];
                Order_summary = String.Format("메뉴 {0}개 · 총 {1}원 · {2}", value.order_menus == null ? 0 : value.order_menus.Count, value.order_amount, value.pay_method);
                string result = "";
                if (value.order_menus != null)
                    for (int i = 0; i < value.order_menus.Count; i++)
                    {
                        result += value.order_menus[i].menu_name;
                        string side = "(";
                        for (int j = 0; j < value.order_menus[i].option_group_list.Count; j++)
                        {
                            for (int k = 0; k < value.order_menus[i].option_group_list[j].options.Count; k++)
                            {
                                side += value.order_menus[i].option_group_list[j].options[k].option_name + "추가";
                                if (j != value.order_menus[i].option_group_list.Count - 1 || k != value.order_menus[i].option_group_list[j].options.Count() - 1)
                                    side += "/";
                                else
                                    side += ")";
                            }
                        }
                        result += side;
                        result += " " + value.order_menus[i].count + "개";
                        if (i != value.order_menus.Count - 1)
                            result += ", ";
                    }
                Order_menus = result;
                Today_count = value.today_count;
                Card_visible = Visibility.Visible;
            }
            else
            {
                //Now_order = new Delivery_App.Class.order();
                Card_visible = Visibility.Hidden;
                alarm_timer.Stop();
                Alert = new SolidColorBrush(Color.FromRgb(48, 49, 64));
            }
            Order_list = temp;
            Order_count = Order_list.Count > 99 ? "99+" : Order_list.Count.ToString();
            Order_count_visible = Order_list.Count == 0 ? Visibility.Hidden : Visibility.Visible;
        }

        public void CancelOrder(string order_cd, DispatcherTimer alarm_timer)
        {
            var temp = Order_list;
            temp.RemoveAll(x => x.order_cd == order_cd);
            if (temp.Count > 0)
            {
                Now_order = temp[0];
                var value = temp[0];
                Order_summary = String.Format("메뉴 {0}개 · 총 {1}원 · {2}", value.order_menus == null ? 0 : value.order_menus.Count, value.order_amount, value.pay_method);
                string result = "";
                if (value.order_menus != null)
                    for (int i = 0; i < value.order_menus.Count; i++)
                    {
                        result += value.order_menus[i].menu_name;
                        string side = "(";
                        for (int j = 0; j < value.order_menus[i].option_group_list.Count; j++)
                        {
                            for (int k = 0; k < value.order_menus[i].option_group_list[j].options.Count; k++)
                            {
                                side += value.order_menus[i].option_group_list[j].options[k].option_name + "추가";
                                if (j != value.order_menus[i].option_group_list.Count - 1 || k != value.order_menus[i].option_group_list[j].options.Count() - 1)
                                    side += "/";
                                else
                                    side += ")";
                            }
                        }
                        result += side;
                        result += " " + value.order_menus[i].count + "개";
                        if (i != value.order_menus.Count - 1)
                            result += ", ";
                    }
                Order_menus = result;
                Today_count = value.today_count;
                Card_visible = Visibility.Visible;
            }
            else
            {
                //Now_order = new Delivery_App.Class.order();
                Card_visible = Visibility.Hidden;
                alarm_timer.Stop();
                Alert = new SolidColorBrush(Color.FromRgb(48, 49, 64));
            }
            Order_list = temp;
            Order_count = Order_list.Count > 99 ? "99+" : Order_list.Count.ToString();
            Order_count_visible = Order_list.Count == 0 ? Visibility.Hidden : Visibility.Visible;
        }

        public void ShowNewOrder(DispatcherTimer alarm_timer)
        {
            RemoveOrder(alarm_timer);
        }

        public void LongerTime()
        {
            Prepare_time = Prepare_time + 5;
        }
        public void ShorterTime()
        {
            Prepare_time = Prepare_time - 5 > 0 ? Prepare_time - 5 : 5;
        }
    }
}