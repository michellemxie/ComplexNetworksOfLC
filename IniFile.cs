using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.IO;

namespace CFG
{
    /// <summary>
    /// Provides methods for reading and writing to an INI file.
    /// </summary>
    public class IniFile
    {
        // The maximum size of the buffers used to retreive data from
        // an ini file.  This value is the maximum allowed by 
        // GetPrivateProfileSectionNames or GetPrivateProfileString.
        private const int MAX_BUFFER = 32767; // 32 KB

        //The path of the file we are operating on.
        private string m_path;

        #region P/Invoke declares

        /// <summary>
        /// A static class that provides the win32 P/Invoke signatures 
        /// used by this class.
        /// </summary>
        /// <remarks>
        /// Note:  In each of the declarations below, we explicitly set CharSet to 
        /// Auto.  By default in C#, CharSet is set to Ansi, which reduces 
        /// performance on windows 2000 and above due to needing to convert strings
        /// from Unicode (the native format for all .Net strings) to Ansi before 
        /// marshalling.  Using Auto lets the marshaller select the Unicode version of 
        /// these functions when available.
        /// </remarks>
        [System.Security.SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer,
                                                                   uint nSize,
                                                                   string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern uint GetPrivateProfileString(string lpAppName,
                                                              string lpKeyName,
                                                              string lpDefault,
                                                              StringBuilder lpReturnedString,
                                                              int nSize,
                                                              string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern uint GetPrivateProfileString(string lpAppName,
                                                              string lpKeyName,
                                                              string lpDefault,
                                                              [In, Out] char[] lpReturnedString,
                                                              int nSize,
                                                              string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetPrivateProfileString(string lpAppName,
                                                             string lpKeyName,
                                                             string lpDefault,
                                                             IntPtr lpReturnedString,
                                                             uint nSize,
                                                             string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetPrivateProfileInt(string lpAppName,
                                                          string lpKeyName,
                                                          int lpDefault,
                                                          string lpFileName);

            //We explicitly enable the SetLastError attribute here for 
            // WritePrivateProfileString returns errors via SetLastError.
            // Failure to set this can result in errors being lost during 
            // the marshal back to managed code.
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool WritePrivateProfileString(string lpAppName,
                                                                string lpKeyName,
                                                                string lpString,
                                                                string lpFileName);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile"/> class.
        /// </summary>
        /// <param name="path">The ini file to read and write from.</param>
        public IniFile(string path)
        {
            m_path = path;
        }

        #region Get Value Methods

        /// <summary>
        /// Gets the value of a setting in an ini file as a <see cref="T:System.String"/>.
        /// </summary>
        /// <param name="appName">The name of the section to read from.</param>
        /// <param name="keyName">The name of the key in section to read.</param>
        /// <param name="defaultValue">The default value to return if the key
        /// cannot be found.</param>
        /// <returns>The value of the key, if found.  Otherwise, returns 
        /// <paramref name="defaultValue"/></returns>
        /// <remarks>
        /// The retreived value must be less than 32KB in length.
        /// </remarks>
        public string GetString(string sectionName,
                                string keyName,
                                string defaultValue)
        {
            StringBuilder retval = new StringBuilder(MAX_BUFFER);

            NativeMethods.GetPrivateProfileString(sectionName,
                                                  keyName,
                                                  defaultValue,
                                                  retval,
                                                  MAX_BUFFER,
                                                  m_path);

            return retval.ToString();
        }

        /// <summary>
        /// Gets the value of a setting in an ini file as a <see cref="T:System.Int16"/>.
        /// </summary>
        /// <param name="appName">The name of the section to read from.</param>
        /// <param name="keyName">The name of the key in section to read.</param>
        /// <param name="defaultValue">The default value to return if the key
        /// cannot be found.</param>
        /// <returns>The value of the key, if found.  Otherwise, returns 
        /// <paramref name="defaultValue"/></returns>
        public int GetInt16(string sectionName,
                            string keyName,
                            short defaultValue)
        {
            int retval = GetInt32(sectionName, keyName, defaultValue);

            return Convert.ToInt16(retval);
        }

        /// <summary>
        /// Gets the value of a setting in an ini file as a <see cref="T:System.Int32"/>.
        /// </summary>
        /// <param name="appName">The name of the section to read from.</param>
        /// <param name="keyName">The name of the key in section to read.</param>
        /// <param name="defaultValue">The default value to return if the key
        /// cannot be found.</param>
        /// <returns>The value of the key, if found.  Otherwise, returns 
        /// <paramref name="defaultValue"/></returns>
        public int GetInt32(string sectionName,
                            string keyName,
                            int defaultValue)
        {
            return NativeMethods.GetPrivateProfileInt(sectionName, keyName, defaultValue, m_path);
        }

        /// <summary>
        /// Gets the value of a setting in an ini file as a <see cref="T:System.Double"/>.
        /// </summary>
        /// <param name="appName">The name of the section to read from.</param>
        /// <param name="keyName">The name of the key in section to read.</param>
        /// <param name="defaultValue">The default value to return if the key
        /// cannot be found.</param>
        /// <returns>The value of the key, if found.  Otherwise, returns 
        /// <paramref name="defaultValue"/></returns>
        public double GetDouble(string sectionName,
                                string keyName,
                                double defaultValue)
        {
            string retval = GetString(sectionName, keyName, "");

            if (retval == null || retval.Length == 0)
            {
                return defaultValue;
            }

            return Convert.ToDouble(retval, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Get Key/Section Names

        /// <summary>
        /// Gets the names of all keys under a specific section in the ini file.
        /// </summary>
        /// <param name="section">
        /// The name of the section to read key names from.
        /// </param>
        /// <returns>An array of key names.</returns>
        /// <remarks>
        /// The total length of all key names in the section must be 
        /// less than 32KB in length.
        /// </remarks>
        public string[] GetKeyNames(string section)
        {
            string[] retval;

            //Allocate a buffer for the returned section names.
            IntPtr ptr = Marshal.AllocCoTaskMem(MAX_BUFFER);

            try
            {
                //Get the section names into the buffer.
                int len = NativeMethods.GetPrivateProfileString(section,
                                                                null,
                                                                null,
                                                                ptr,
                                                                MAX_BUFFER,
                                                                m_path);

                if (len == 0)
                {
                    //Free the buffer
                    retval = new string[0];
                }
                else
                {
                    //Convert the buffer back into a string.  Decrease the length 
                    //by 1 so that we remove the second null off the end.
                    string buff = Marshal.PtrToStringAuto(ptr, len - 1);

                    //Parse the buffer into an array of strings by searching for nulls.
                    retval = buff.Split('\0');
                }
            }
            finally
            {
                //Free the buffer
                Marshal.FreeCoTaskMem(ptr);
            }

            return retval;
        }

        /// <summary>
        /// Gets the names of all sections in the ini file.
        /// </summary>
        /// <returns>An array of section names.</returns>
        /// <remarks>
        /// The total length of all section names in the section must be 
        /// less than 32KB in length.
        /// </remarks>
        public string[] GetSectionNames()
        {
            string[] retval;

            //Allocate a buffer for the returned section names.
            IntPtr ptr = Marshal.AllocCoTaskMem(MAX_BUFFER);

            try
            {
                //Get the section names into the buffer.
                int len = NativeMethods.GetPrivateProfileSectionNames(ptr, MAX_BUFFER, m_path);

                if (len == 0)
                {
                    retval = new string[0];
                }
                else
                {
                    //Convert the buffer back into a string.  Decrease the length by 1 so 
                    //that we remove the second null off the end.
                    string buff = Marshal.PtrToStringAuto(ptr, len - 1);

                    //Parse the buffer into an array of strings by searching for nulls.
                    retval = buff.Split('\0');
                }
            }
            finally
            {
                //Free the buffer
                Marshal.FreeCoTaskMem(ptr);
            }

            return retval;
        }

        #endregion

        #region Write Methods

        /// <summary>
        /// Writes a <see cref="T:System.String"/> value to the ini file.
        /// </summary>
        /// <param name="sectionName">The name of the section to write to .</param>
        /// <param name="keyName">The name of the key to write to.</param>
        /// <param name="value">The string value to write</param>
        /// <exception cref="Win32Exception">
        /// The write failed.
        /// </exception>
        public void WriteValue(string sectionName, string keyName, string value)
        {
            if (!NativeMethods.WritePrivateProfileString(sectionName, keyName, value, m_path))
            {
                throw new System.ComponentModel.Win32Exception();
            }
        }

        /// <summary>
        /// Writes an <see cref="T:System.Int16"/> value to the ini file.
        /// </summary>
        /// <param name="sectionName">The name of the section to write to .</param>
        /// <param name="keyName">The name of the key to write to.</param>
        /// <param name="value">The value to write</param>
        /// <exception cref="Win32Exception">
        /// The write failed.
        /// </exception>
        public void WriteValue(string sectionName, string keyName, short value)
        {
            WriteValue(sectionName, keyName, (int)value);
        }

        /// <summary>
        /// Writes an <see cref="T:System.Int32"/> value to the ini file.
        /// </summary>
        /// <param name="sectionName">The name of the section to write to .</param>
        /// <param name="keyName">The name of the key to write to.</param>
        /// <param name="value">The value to write</param>
        /// <exception cref="Win32Exception">
        /// The write failed.
        /// </exception>
        public void WriteValue(string sectionName, string keyName, int value)
        {
            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes an <see cref="T:System.Single"/> value to the ini file.
        /// </summary>
        /// <param name="sectionName">The name of the section to write to .</param>
        /// <param name="keyName">The name of the key to write to.</param>
        /// <param name="value">The value to write</param>
        /// <exception cref="Win32Exception">
        /// The write failed.
        /// </exception>
        public void WriteValue(string sectionName, string keyName, float value)
        {
            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes an <see cref="T:System.Double"/> value to the ini file.
        /// </summary>
        /// <param name="sectionName">The name of the section to write to .</param>
        /// <param name="keyName">The name of the key to write to.</param>
        /// <param name="value">The value to write</param>
        /// <exception cref="Win32Exception">
        /// The write failed.
        /// </exception>
        public void WriteValue(string sectionName, string keyName, double value)
        {
            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        private string[] ReadFile( string filepath)
        {
            //string file;
            //file = Directory.GetParent(filepath).FullName;
            // filepath = file + @"\图2-8-1-Hp.csv"; 

            try
            {
                if (File.Exists(filepath))
                {
                    //char[] c_str1 = new char[51];
                    //char[] c_str2 = new char[51];
                    //string c_str;
                    //StreamReader s;
                    ////s = new StreamReader(filepath);
                    //s.Read(c_str1, 0, 49);
                    //s.Read(c_str2, 51, 99);
                    string[] c_str = File.ReadAllLines(filepath);
                    //string[] c_str = SmartCardLib.DecryptFile(filepath, RouteServerProgram.strAes);
                    foreach (string s in c_str)
                    {
                        char[] c_str1 = s.ToCharArray();
                    }

                    return c_str;
                }

                else
                {
                    Console.Write("the data file doesn't exist!");
                    return null;
                }
            }
            catch (SystemException e)
            {
                Console.Write(e.Message.ToString());
                return null;
            }
        }

        //调用数据初始化函数进行样本点数据数组初始化
        /// <summary>
        /// 调用数据初始化函数进行样本点数据数组初始化
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public double[,] DataInitial(string filepath)   
        {
            int flag;
            int i,j;
            string[] s = ReadFile(filepath);   //将全部文本内容读入内存中
            if (s != null)
            {
                try
                {
                    string[] data = s[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);  //按照“，”分隔，将字符串分离成字符数组
                    //获得所取得的x轴的参数个数
                    int length = s.Length;
                    //xLength = length;
                    //横向y轴的维数
                    int number = data.Length;
                    //yLength = number;

                    double[,] y = new double[length-1, number];

                    for (i = 1; i < s.Length; i++)
                    {
                        flag = 0;
                        data = s[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (j = 0; j < data.Length; j++)
                        {
                            if (i == 0 && j == 0)
                                y[i-1, j] = 0.0;
                            else
                            {
                                if (data[j] != "")
                                {
                                    flag++;
                                    y[i-1, j] = Convert.ToDouble(data[j].ToString());
                                }
                                else
                                    y[i-1, j] = Convert.ToDouble(data[flag].ToString());
                            }
                        }
                    }
                    return (y);
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e.Message.ToString());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }

}
