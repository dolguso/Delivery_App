using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delivery_App
{
    public class PrinterFunction
    {
        static public void btn_PrintReceipt_Click(object sender, EventArgs e, List<Class.order> order_list)
        {
            try
            {
                for (int i = 0; i < order_list.Count; i++)
                {
                    if (!Delivery_App.MainWindow.printed_list.Contains(order_list[i].order_cd))
                    {
                        // This receipt is written by Korean,
                        // You can change as your own language
                        string printStr = "";
                        printStr += "사전결제 여부 : X       [배달]    [고객용]";
                        printStr += "\n------------------------------------------\n";
                        printStr += (order_list[i].shop_name + "\n");
                        printStr += "\n주문일시 : " + order_list[i].order_dtime;
                        printStr += "\n주문번호 : " + order_list[i].order_cd;
                        printStr += "\n배달주소";
                        printStr += "\n" + order_list[i].order_addr_jibun;
                        printStr += "\n(도로명) " + order_list[i].order_addr_doro;
                        //printStr += "\n"+order_list[i].;
                        printStr += "\n                                          ";
                        printStr += "\n전화번호";
                        printStr += "\n" + order_list[i].order_safetel != "" ? order_list[i].order_safe_telno : order_list[i].order_tel;
                        printStr += "\n" + order_list[i].order_safetel != "" ? "* 안심번호는 주문접수 후 최대 3시간 유효 *" : "";
                        printStr += "\n주문요청사항";
                        printStr += "\n(사장님께) " + order_list[i].order_reqmsg;
                        printStr += "\n(라이더님께) " + order_list[i].order_delivmsg;
                        printStr += "\n\n";
                        printStr += "------------------------------------------\n";
                        printStr += "메뉴                    수량          금액\n"; // 메뉴 24칸, 수량 6칸, 금액 12칸
                        printStr += "------------------------------------------\n";
                        for (int j = 0; j < order_list[i].order_menus.Count; j++)
                        {
                            string menu_line = "\n" + right_pad(24, order_list[i].order_menus[j].menu_name) + order_list[i].order_menus[j].count.ToString() + left_pad(18, String.Format("{0:n0}", order_list[i].order_menus[j].menu_price_amount), order_list[i].order_menus[j].count.ToString());
                            printStr += menu_line;
                            for (int k = 0; k < order_list[i].order_menus[j].option_group_list.Count; k++)
                            {
                                var temp_optiongroup = order_list[i].order_menus[j].option_group_list[k];
                                for (int l = 0; l < order_list[i].order_menus[j].option_group_list[k].options.Count; l++)
                                {
                                    var temp_option = order_list[i].order_menus[j].option_group_list[k].options[l];
                                    string option_line = "\n  " + right_pad(22, (temp_optiongroup.option_group_name + " - " + temp_option.option_name)) + left_pad(18, (String.Format("{0:n0}", temp_option.option_amount)));
                                    printStr += option_line;
                                }
                            }
                        }
                        printStr += "\n------------------------------------------";
                        printStr += "\n" + right_pad(30, "총 결제금액") + left_pad(12, String.Format("{0:n0}", order_list[i].order_amount));
                        printStr += "\n" + right_pad(12, "결제방법") + left_pad(30, order_list[i].pay_method);
                        printStr += "\n고객정보를 배달목적 외 사용하거나 보관, 공개할 경우 법적처벌을 받을 수 있습니다.";
                        printStr += "\n원산지정보 : " + order_list[i].shop_origin;
                        // Blank String to Print out properly
                        printStr += "                                                                                                                  \n";
                        printStr += "                                                                                                                  \n";
                        printStr += "                                                                                                                  \n";
                        printStr += "                                                                                                                  \n";

                        RawPrinterHelper.SendStringToPrinter(Program.printerName, printStr);
                        RawPrinterHelper.PartialCut();
                        Delivery_App.MainWindow.printed_list.Add(order_list[i].order_cd);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Print Receipt");
            }
        }

        static private string left_pad(int length, string input_string, string sub_string = "")
        {
            return "".PadRight(length - Encoding.Default.GetBytes(input_string + sub_string).Length) + input_string;
        }
        static private string right_pad(int length, string input_string, string sub_string = "")
        {
            return input_string + "".PadRight(length - Encoding.Default.GetBytes(input_string + sub_string).Length);
        }
        static public void print_receipt(List<Class.order> order_list)
        {
            try
            {
                for (int i = 0; i < order_list.Count; i++)
                {
                    if (!Delivery_App.MainWindow.printed_list.Contains(order_list[i].order_cd))
                    {
                        // This receipt is written by Korean,
                        // You can change as your own language
                        string printStr = "\n";
                        printStr += "사전결제 여부 : X       [배달]    [고객용]";
                        printStr += "\n------------------------------------------\n";
                        printStr += (order_list[i].shop_name + "\n");
                        printStr += "\n주문일시 : " + order_list[i].order_dtime;
                        printStr += "\n주문번호 : " + order_list[i].order_cd;
                        printStr += "\n배달주소";
                        printStr += "\n" + order_list[i].order_addr_jibun;
                        printStr += "\n(도로명) " + order_list[i].order_addr_doro;
                        //printStr += "\n"+order_list[i].;
                        printStr += "\n                                          ";
                        printStr += "\n전화번호";
                        printStr += "\n" + order_list[i].order_safetel != "" ? order_list[i].order_safe_telno : order_list[i].order_tel;
                        printStr += "\n" + order_list[i].order_safetel != "" ? "* 안심번호는 주문접수 후 최대 3시간 유효 *" : "";
                        printStr += "\n주문요청사항";
                        printStr += "\n(사장님께) " + order_list[i].order_reqmsg;
                        printStr += "\n(라이더님께) " + order_list[i].order_delivmsg;
                        printStr += "\n\n";
                        printStr += "------------------------------------------\n";
                        printStr += "메뉴                    수량          금액\n"; // 메뉴 24칸, 수량 6칸, 금액 12칸
                        printStr += "------------------------------------------\n";
                        for (int j = 0; j < order_list[i].order_menus.Count; j++)
                        {
                            string menu_line = "\n" + right_pad(24, order_list[i].order_menus[j].menu_name) + order_list[i].order_menus[j].count.ToString() + left_pad(18, String.Format("{0:n0}", order_list[i].order_menus[j].menu_price_amount), order_list[i].order_menus[j].count.ToString());
                            printStr += menu_line;
                            for (int k = 0; k < order_list[i].order_menus[j].option_group_list.Count; k++)
                            {
                                var temp_optiongroup = order_list[i].order_menus[j].option_group_list[k];
                                for (int l = 0; l < order_list[i].order_menus[j].option_group_list[k].options.Count; l++)
                                {
                                    var temp_option = order_list[i].order_menus[j].option_group_list[k].options[l];
                                    string option_line = "\n  " + right_pad(22, (temp_optiongroup.option_group_name + " - " + temp_option.option_name)) + left_pad(18, (String.Format("{0:n0}", temp_option.option_amount)));
                                    printStr += option_line;
                                }
                            }
                        }
                        printStr += "\n------------------------------------------";
                        printStr += "\n" + right_pad(30, "총 결제금액") + left_pad(12, String.Format("{0:n0}", order_list[i].order_amount));
                        printStr += "\n" + right_pad(12, "결제방법") + left_pad(30, order_list[i].pay_method);
                        printStr += "\n고객정보를 배달목적 외 사용하거나 보관, 공개할 경우 법적처벌을 받을 수 있습니다.";
                        printStr += "\n원산지정보 : " + order_list[i].shop_origin;
                        // Blank String to Print out properly
                        printStr += "                                                                                                                  \n";
                        printStr += "                                                                                                                  \n";
                        printStr += "                                                                                                                  \n";
                        printStr += "                                                                                                                  \n";

                        RawPrinterHelper.SendStringToPrinter(Program.printerName, printStr);
                        RawPrinterHelper.PartialCut();
                        Delivery_App.MainWindow.printed_list.Add(order_list[i].order_cd);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Print Receipt");
            }
        }

    }
}
