using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery_App.Class
{
    public class order
    {
        public int today_count { get; set; }
        public string order_cd { get; set; }
        public string shop_name { get; set; }
        public string order_disposable { get; set; }
        public string order_dtime { get; set; }
        public string level_gubun { get; set; }
        public string order_delivgb { get; set; }
        public List<menu> order_menus { get; set; }
        public int order_tip_amount { get; set; }
        public int order_amount { get; set; }
        public string order_status { get; set; }
        public string pay_method { get; set; }
        public string order_addr_jibun { get; set; }
        public string order_addr_doro { get; set; }
        public string order_safetel { get; set; }
        public string order_safe_telno { get; set; }
        public string order_tel { get; set; }
        public string order_reqmsg { get; set; }
        public string order_delivmsg { get; set; }
        public string shop_origin { get; set; }
    }
    public class menu
    {
        public string menu_price_cd { get; set; }
        public int menu_price_amount { get; set; }
        public int count { get; set; }
        public List<option_group> option_group_list { get; set; }
        public string menu_price_name { get; set; }
        public string menu_cd { get; set; }
        public string menu_name { get; set; }
    }

    public class option_group
    {
        public string option_group_name { get; set; }
        public string option_group_cd { get; set; }
        public List<option> options { get; set; }
    }

    public class option
    {
        public string option_cd { get; set; }
        public string option_name { get; set; }
        public int option_amount { get; set; }
    }

    public class store
    {
        //public string shop_cd { get; set; }
        public int istmpstop { get; set; }
        public int isstop { get; set; }
    }
}
