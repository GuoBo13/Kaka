using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 运算符测试
{
    class wal
    {

        /// <summary>
        /// 获取负数的补码
        /// </summary>
        /// <param name="OriginalCode"></param>
        /// <returns></returns>
        private sbyte ConvertComplementCode(byte _code)
        {
            byte temp = (byte)(_code << 1);
            byte num = (byte)(temp >> 1);
            sbyte ret = (sbyte)(num ^ 0x7f + 1);
            return ret;
        }
        /// <summary>
        /// 解析WAL日志中单元内容
        /// </summary>
        /// <param name="array"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string AnalysisRowData(byte[] array, int data, int offset, out int num)
        {
            string content = "";
            int len = 0;
            if (data == 0)
            {
                content = null;
                num = 0;
            }
            else if (data >= 1 && data <= 4)
            {
                //byte[] size = new byte[data];
                //size =array.Skip(offset).Take(data).Reverse<byte>().ToArray<byte>();
                if (data == 1)
                {
                    var _msgSvrId = (array.Skip(offset).Take(1)).ElementAt(0);
                    if (_msgSvrId > 127)
                    {
                        content = Convert.ToString(ConvertComplementCode(_msgSvrId));
                    }
                    else
                    {
                        content = _msgSvrId.ToString();
                    }
                }
                else if (data == 2)
                {
                    content = BitConverter.ToInt16(array.Skip(offset).Take(2).Reverse<byte>().ToArray<byte>(), 0).ToString();
                }
                else if (data == 3)
                {
                    byte[] temp = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
                    temp = array.Skip(offset).Take(3).ToArray<byte>();
                    for (int ii = 1; ii < temp.Length; ii++)
                    {
                        temp[ii] = array.Skip(offset).Take(ii).ElementAt(ii - 1);
                    }
                    Array.Reverse(temp);
                    content = BitConverter.ToInt32(temp, 0).ToString();
                }
                else
                {
                    content = BitConverter.ToInt32(array.Skip(offset).Take(4).Reverse<byte>().ToArray<byte>(), 0).ToString();
                }
                num = data;
            }
            else if (data == 5)
            {
                byte[] temp = new byte[8];
                for (int jj = 0; jj < temp.Length; jj++)
                {
                    temp[jj] = 0;
                }
                for (int k = 2; k < temp.Length; k++)
                {
                    temp[k] = array.Skip(offset).Take(k - 1).ElementAt(k - 2);
                }
                Array.Reverse(temp);
                content = BitConverter.ToInt64(temp, 0).ToString();
                num = 6;
            }
            else if (data == 6)
            {
                content = BitConverter.ToInt64(array.Skip(offset).Take(8).Reverse<byte>().ToArray<byte>(), 0).ToString();
                num = 8;
            }
            //不确定是不是这样转，IEEE浮点数的转换需要确定
            else if (data == 7)
            {
                content = BitConverter.ToDouble(array.Skip(offset).Take(8).Reverse<byte>().ToArray<byte>(), 0).ToString();
                num = 8;
            }
            else if (data == 8 || data == 9 || data == 10 || data == 11)
            {
                content = "1";
                num = 0;
            }
            else
            {
                if (data % 2 == 0 && data >= 12)
                {
                    len = (data - 12) / 2;
                    byte[] temp = array.Skip(offset).Take(len).ToArray<byte>();
                    content = Encoding.UTF8.GetString(temp);
                }
                else
                {
                    if (data % 2 == 1 && data >= 13)
                    {
                        len = (data - 13) / 2;
                        byte[] temp = array.Skip(offset).Take(len).ToArray<byte>();
                        content = Encoding.UTF8.GetString(temp);
                    }
                }
                num = len;
            }
            return content;
        }
    }
}
