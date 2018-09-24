using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace CFG
{
    class Program
    {
        //private DateTime time;
        /// <summary>
        /// 分布式计算的同步step
        /// </summary>
        public int stepForShow = 0;

        //private Allocation allo = new Allocation();
        /// <summary>
        /// 主程序入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Allocation allo = new Allocation();
            DateTime time;
            int stepForShow = 0;
            //string f = "h:\\programming Data\\Data\\G08_T40A100TM100\\NO1";
            //G01_T25A100;G01_T25A125;G01_T25A150;G01_T25A200;
            //G02_T30A100;G02_T30A125;G02_T30A125;G02_T30A150;G02_T30A200;
            //G04_T30A125;G03_T50A150;G03_T50A200;
            //G05_T40A125;G05_T40A150;G05_T40A200;
            //G05_T40A100;G06_T50A150;G07_T35A100
            //G08_T40A100TM100;G09_T50A100TM100;G10_T60A100TM100;G01_T20A50
            //G11_T40A100TM150;G12_T50A100TM150;G13_T60A100TM150
            //G14_T40A100TM200;G15_T50A100TM200;G16_T60A100TM200;
            //G17_T40A70TM100;G18_T50A70TM100;G19_T60A70TM100;
            //G20_T40A50TM100;G21_T50A50TM100;G22_T60A50TM100;
            //G23_T40A100TM150;G24_T50A100TM150;G25_T60A100TM150;
            //G26_T40A100TM200;G27_T50A100TM200;G28_T60A100TM200;
            for (int num = 1; num <= 10; num++)
            {
                string f = "h:\\programming Data\\Data\\CMP\\G04_T50A200\\NO" + num.ToString();
                allo.center.lat = 400;
                allo.center.lot = 300;
                allo.Initial_Task(f);
                allo.Initial_Agents(f);
                int step;
                int formStep, nowStep;
                int interval = 0; //时间间隔
                double fls = 0;
                //设置基准时间
                time = System.DateTime.Now;
                step = time.Hour * 3600 + time.Minute * 60 + time.Second;
                formStep = step;
                do
                {
                    time = System.DateTime.Now;

                    nowStep = time.Hour * 3600 + time.Minute * 60 + time.Second;
                    interval = nowStep - formStep;
                    if (interval >= 1)
                    {
                        stepForShow = nowStep - step;
                        allo.Agent_Assignment(stepForShow);
                        //Repaint();                    
                    }
                    formStep = nowStep;
                } while (allo.task_Pending.Count != 0 || allo.task_Waiting.Count != 0 || allo.task_UnderGoing.Count != 0);
                fls = (allo.distance.ablityX + allo.distance.ablityY) / (allo.totalAblity.ablityX + allo.totalAblity.ablityY);
                //allo.TaskInfo += fls.ToString();
                #region 普通实验数据对比
                allo.TaskInfo += "\n completed task number is " + allo.task_Complete.Count.ToString() +
                                 "\n uncompleted task number is " + allo.task_UnComplete.Count.ToString();
                //eLa;DC
                //File.WriteAllText("h:\\programming Data\\Result\\自适应试验数据分析\\对比组试验\\SAC\\G21_T50A50TM100\\NO3\\taskResult对比01.csv", allo.TaskInfo);
                //File.WriteAllText("h:\\programming Data\\Result\\自适应试验数据分析\\对比组试验\\SAC\\G21_T50A50TM100\\NO3\\agentResult对比01.csv", allo.AgentInfo);
                File.WriteAllText("D:\\个人DOC\\programsCodes\\paper\\20180523 ComplexNet in SelfAdaption\\result\\New\\LR\\G04_T50A200\\NO" + num.ToString() + "\\taskResult.csv", allo.TaskInfo);
                File.WriteAllText("D:\\个人DOC\\programsCodes\\paper\\20180523 ComplexNet in SelfAdaption\\result\\New\\LR\\G04_T50A200\\NO" + num.ToString() + "\\agentResult.csv", allo.AgentInfo);
                Console.WriteLine("End");
                #endregion
                #region SMSMA对比MSMA
                ////File.WriteAllText("h:\\programming Data\\Result\\自适应试验数据分析\\对比组试验\\SAC\\G08_T40A100TM100\\NO5\\taskResult对比01.csv", allo.TaskInfo);
                ////File.WriteAllText("h:\\programming Data\\Result\\自适应试验数据分析\\对比组试验\\SAC\\G08_T40A100TM100\\NO5\\agentResult对比01.csv", allo.AgentInfo);
                //File.WriteAllText("h:\\programming Data\\Result\\自适应试验数据分析\\Self-Adaption in Net\\SAC\\G08_T40A100TM100\\NO10\\taskResult01.csv", allo.TaskInfo);
                //File.WriteAllText("h:\\programming Data\\Result\\自适应试验数据分析\\Self-Adaption in Net\\SAC\\G08_T40A100TM100\\NO10\\agentResult01.csv", allo.AgentInfo);
                #endregion
                Console.WriteLine("End");
                Console.WriteLine(fls.ToString());
                // Console.ReadLine();
            }
        }     
     }
}
