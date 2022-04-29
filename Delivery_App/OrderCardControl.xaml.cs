using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media.Animation;

namespace Delivery_App
{
    /// <summary>
    /// OrderCardControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OrderCardControl : UserControl
    {
        public int order_predtime = 20;
        public string order_cd = "";
        public int order_index = -1;
        Delivery_App.Model.ViewModel myModel;
        DispatcherTimer alarm;
        System.Windows.Window get_window;
        System.Windows.Controls.Grid noti_grid;
        Storyboard sb;

        public OrderCardControl(Delivery_App.Model.ViewModel viewModel, DispatcherTimer alarm_timer, System.Windows.Window input_window = null, System.Windows.Controls.Grid input_noti_grid = null)
        {
            InitializeComponent();
            myModel = viewModel;
            this.DataContext = viewModel;
            alarm = alarm_timer;
            get_window = input_window;
            noti_grid = input_noti_grid;
        }

        JObject cancel_order()
        {
            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myModel.baseUrl);
            request.Headers.Add("Authorization", myModel.Jwt);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Method = "POST";
            request.Timeout = 30 * 1000; // 30초
            String postData = "";
            byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);
            request.ContentLength = sendData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);
            requestStream.Close();

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            return JObject.Parse(responseText);
        }

        JObject accept_order()
        {
            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myModel.baseUrl);
            request.Headers.Add("Authorization", myModel.Jwt);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Method = "POST";
            request.Timeout = 30 * 1000; // 30초
            String postData = String.Format("order_cd={0}&order_predtime={1}", myModel.Now_order.order_cd, myModel.Prepare_time);
            byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);
            request.ContentLength = sendData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);
            requestStream.Close();

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            //cancel_order();
            return JObject.Parse(responseText);
        }

        JObject reject_order()
        {
            string responseText = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myModel.baseUrl);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Headers.Add("Authorization", myModel.Jwt);
            request.Accept = "application/json";
            request.Method = "POST";
            request.Timeout = 30 * 1000; // 30초
            String postData = ""; // 추후 거절 사유 선택 팝업창 제작
            byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);
            request.ContentLength = sendData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);
            requestStream.Close();

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            return JObject.Parse(responseText);
        }

        private void Btn_accept_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            JObject result = accept_order();
            if (result["code"].ToString() == "1")
                myModel.ShowNewOrder(alarm);
        }

        private void Btn_reject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            JObject result = reject_order();
            if (result["code"].ToString() == "1")
                myModel.ShowNewOrder(alarm);
        }

        private void Btn_longer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myModel.LongerTime();
        }

        private void Btn_shorter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myModel.ShorterTime();
        }
        private void Btn_detail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            get_window.ShowInTaskbar = true;
            get_window.Show();
            noti_grid.Visibility = System.Windows.Visibility.Hidden;
        }
        public static readonly RoutedEvent CustomTestEvent = EventManager.RegisterRoutedEvent("CustomTest", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OrderCardControl));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler CustomTest
        {
            add { AddHandler(CustomTestEvent, value); }
            remove { RemoveHandler(CustomTestEvent, value); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            double targetY = alarm_card_body.ActualHeight;

            sb = new Storyboard();
            var ta = new ThicknessAnimation();
            ta.BeginTime = new TimeSpan(0);
            ta.SetValue(Storyboard.TargetNameProperty, "alarm_card_body");
            Storyboard.SetTargetProperty(ta, new PropertyPath(MarginProperty));

            ta.From = new Thickness(0, targetY, 0, 0);
            ta.To = new Thickness(0, 0, 0, 0);
            ta.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            sb.Children.Add(ta);
            sb.Begin(this);
            alarm_card_body.IsVisibleChanged += alarm_card_body_IsVisibleChanged;
        }

        private void alarm_card_body_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            sb.Begin(this);
        }
    }
}
