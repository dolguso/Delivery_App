using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Delivery_App
{
    class Notify
    {
        static public void setNotify(Delivery_App.MainWindow mainWindow, System.Windows.Controls.Grid input_noti_grid)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon(Directory.GetCurrentDirectory() + "/Icon/Delivery_App.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "영업중";
            notifyIcon.DoubleClick += delegate (object sender, EventArgs eventArgs)
            {
                mainWindow.Show();
                mainWindow.ShowInTaskbar = true;
                input_noti_grid.Visibility = System.Windows.Visibility.Hidden;

            };
            ContextMenu menu = new ContextMenu();
            MenuItem open_menu = new MenuItem();
            open_menu.Text = "열기";
            MenuItem close_menu = new MenuItem();
            close_menu.Text = "종료";

            open_menu.Click += delegate (object sender, EventArgs eventArgs)
            {
                //mainWindow.WindowState = mainWindow.pre_state;
                mainWindow.ShowInTaskbar = true;
                mainWindow.Show();
                input_noti_grid.Visibility = System.Windows.Visibility.Hidden;
            };

            close_menu.Click += delegate (object sender, EventArgs eventArgs)
            {
                notifyIcon.Dispose();
                System.Windows.Application.Current.Shutdown();
            };

            menu.MenuItems.Add(open_menu);
            menu.MenuItems.Add(close_menu);
            notifyIcon.ContextMenu = menu;
        }
    }
}
