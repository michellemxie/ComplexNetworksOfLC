using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace CFG
{
    class Allocation
    {
        public List<Task> task_Pending;
        /// <summary>
        /// 当前等待被分配的任务
        /// </summary>
        public List<Task> task_Waiting;
        /// <summary>
        /// 已分配但是未完成的任务
        /// </summary>
        public List<Task> task_UnderGoing;
        /// <summary>
        /// 完成的任务
        /// </summary>
        public List<Task> task_Complete;
        /// <summary>
        /// 未能完成分配的任务
        /// </summary>
        public List<Task> task_UnComplete;
        /// <summary>
        /// agent的全集 
        /// </summary>
        public List<Agent> agentAll;
        /// <summary>
        ///  //主程序启动目录的上一级目录
        /// </summary>
        private static string applicationStartupPath = "";
        /// <summary>
        /// 文件读取
        /// </summary>
        private IniFile file_container;
        /// <summary>
        ///任务完成记录
        /// </summary>
        public string TaskInfo="";
        /// <summary>
        /// 标记任务区域中心位置
        /// </summary>
        public Location center;
        /// <summary>
        /// 智能体状态记录
        /// </summary>
        public string AgentInfo = "";
        public ablity distance = new ablity();
        public ablity totalAblity = new ablity();
        /// <summary>
        /// 自适应调整trigger
        /// </summary>
        private bool flagAdaption = false;
        /// <summary>
        /// 初始化任务信息
        /// </summary>
        public void Initial_Task(string file)
        {
            applicationStartupPath = file;
            file_container = new IniFile(applicationStartupPath);
            double[,] taskInfo = file_container.DataInitial(applicationStartupPath + Properties.Settings.Default.Task_Info);
            //将任务信息初始化
            int row = taskInfo.GetLength(0);
            task_Pending = new List<Task>(row);
            for (int i = 0; i < row; i++)
            {
                Task task = new Task();
                task.requirement = new ablity();
                //将信息全部读取至任务列表中
                task.ID = (int)taskInfo[i, 0];
                task.lat = (float)taskInfo[i, 1];
                task.lot = (float)taskInfo[i, 2];
                task.requirement.ablityX = (float)taskInfo[i, 3];
                task.requirement.ablityY = (float)taskInfo[i, 4];
                task.reward = (int)taskInfo[i, 5];
                task.call_time = (int)taskInfo[i, 6];
                task.deadline = (int)taskInfo[i, 7];
                task.period_time = 10;
                task.status = Task.taskStatus.idle;
                task.arrive_time = 0;
                task.excuting_counter = 0;
                task.pre_members = new List<CoaMembers>(0);
                task_Pending.Add(task);
            }
            task_UnderGoing = new List<Task>(0);
            task_Complete = new List<Task>(0);
            task_Waiting = new List<Task>(0);
            task_UnComplete = new List<Task>(0);
        }

        /// <summary>
        /// 将agent列表初始化，并所有的agent都标注为空闲状态
        /// 同时将agent的邻居列表进行初始化
        /// </summary>
        public void Initial_Agents(string file)
        {
            applicationStartupPath = file;
            file_container = new IniFile(applicationStartupPath);
            double[,] agentInfo = file_container.DataInitial(applicationStartupPath + Properties.Settings.Default.Agent_Info);
            //将任务信息初始化
            int row = agentInfo.GetLength(0);
            agentAll = new List<Agent>(agentInfo.Length / row);
            System.Timers.Timer t = new System.Timers.Timer();
            for (int i = 0; i < row; i++)
            {
                Agent agent = new Agent();
                agent.cnaM = new CNAMe();
                agent.LocalCNA = new ComplexNetAttrLocal();
                agent.abilityA = new ablity();
                agent.ID = (int)agentInfo[i, 0];
                agent.curtLocattion.lat = (float)agentInfo[i, 1];
                agent.curtLocattion.lot = (float)agentInfo[i, 2];
                agent.abilityA.ablityX = (float)agentInfo[i, 3];
                agent.abilityA.ablityY = (float)agentInfo[i, 4];
                agent.speed = 10;//(float)agentInfo[i, 5];
                agent.statusInCoalition = agentStatusInCoalition.IdleAgent;

                agent.potential_Offers = new List<Offers>(0);
                agent.fuelCost = 1f;
                agent.comRadio = 100;
                agent.cruiseSpeed = 5;
                agentAll.Add(agent);
            }
           
        }

        /// <summary>
        /// 对全体智能体进行邻域智能体集合计算
        /// </summary>
        /// <param name="agent_list">目标智能体集合</param>
        private void NeighborHoodUpdate(List<Agent> agent_list)
        {
            flagAdaption = false;
            if (agent_list != null && agent_list.Count != 0)
            {
                for (int i = 0; i < agent_list.Count; i++)
                {
                    ///初始化智能体时，对其进行邻居节点初始化
                    agent_list[i].list_Neighbors = new List<Neighbor>(0);
                    //具化智能体邻居集
                    agent_list[i].NeighborhoodCoalitionConstruct(agentAll, ref agentAll[i].list_Neighbors);
                    ///初始化智能体的策略空间
                    agent_list[i].strategy_List = new List<Strategy>(0);
                }

                ///初始化次邻居节点
                for (int i = 0; i < agent_list.Count; i++)
                {
                    ///初始化每个智能体的次邻居节点集合
                    agent_list[i].list_subNeighbors = new List<Neighbor>(0);
                    ///具化智能体的次邻居集合
                    agent_list[i].CoalitionOfSubNeighborsConstruct(ref agentAll[i].list_subNeighbors);
                }
            }
            //将信息同步，并计算智能体的neighborhood Coalition值
            for (int i = 0; i < agent_list.Count; i++)
            {
                foreach (Neighbor nb in agent_list[i].list_Neighbors)
                {
                    foreach (Agent a in agent_list)
                    {
                        if (nb.agent.ID == a.ID)
                        {
                            nb.agent = a;
                        }
                    }
                }
                foreach (Neighbor nb in agent_list[i].list_subNeighbors)
                {
                    foreach (Agent a in agent_list)
                    {
                        if (nb.agent.ID == a.ID)
                        {
                            nb.agent = a;
                        }
                    }
                }
                //计算Neighborhood Coalition的Utility
                agent_list[i].agentNetValue = agent_list[i].valueOfNeighborhoodCoalition(agent_list[i]);
                //计算当前智能体A网络节点的度，
                agent_list[i].cnaM = agent_list[i].GetDC(agent_list[i],agentAll.Count);
                //并计算当前节点的局域网复杂网络参数（熵）
                ComplexNetAttrLocal newCNP = new ComplexNetAttrLocal();
                newCNP = agent_list[i].getLocalCPNAttr(agent_list[i], agentAll.Count);
                agent_list[i].LocalCNA = newCNP;
            }
        }
        /// <summary>
        /// 任务分配主体函数
        /// </summary>
        public void Agent_Assignment(int stepForShow)
        {
            //
            flagAdaption = false;
            NeighborHoodUpdate(agentAll);
            //对任务按照时间进行分类并进行预处理
            Task_Cateloge(stepForShow);
            //对于进行分配的任务找到其leader进行offer分发
            for (int i = 0; i < task_Waiting.Count; i++)
            {
                if (task_Waiting[i].potential_leader != null)
                {
                    for (int j = 0; j < agentAll.Count; j++)
                    {
                        ///获得任务通知的
                        if (task_Waiting[i].potential_leader.ID == agentAll[j].ID)
                        {
                            //每个任务的领导者
                            //智能体a将接受的任务的合同发送给所有邻接点以及次邻居节点（假设是由不能接受任务的）
                            //将该智能体提交到对应任务的备选智能体集合中 
                            //j是当前智能体的索引号
                            agentAll[j].SendOffers(ref agentAll, j, task_Waiting[i], stepForShow);                           
                        }
                    }
                }
            }
           
            ///智能体进行合约选择
            if (task_Waiting.Count != 0) //如果有需要分配的任务，则进行分配
            {
                for (int k = 0; k < agentAll.Count; k++)
                {
                    if (agentAll[k].potential_Offers != null && agentAll[k].potential_Offers.Count > 0)
                    { 
                        //智能体进行合约选择，同时将确定选择的智能体放入对应的任务的预选队列
                        agentAll[k].OfferChooseAndTransfer(ref task_Waiting, ref agentAll, k);
                    }
                }
            }
            for (int i = 0; i < task_Waiting.Count; i++)
            {
                //对联盟进行优化选择          
                if (task_Waiting[i].pre_members.Count != 0 && task_Waiting[i].CoaAbilityCheck(task_Waiting[i].pre_members))
                {
                    int mark = task_Waiting[i].pre_members.Count;
                    if (task_Waiting[i].CoalitionOptimization(task_Waiting[i], stepForShow))
                    {
                        //如果任务i的联盟完成优化构建
                        if (task_Waiting[i].CoaAbilityCheck(task_Waiting[i].coaMembers))
                        {
                            //将其联盟成员的智能体状态进行修改
                            task_Waiting[i].startTime = stepForShow;
                            List<CoaMembers> coaMemberList = new List<CoaMembers>(0);
                            foreach (CoaMembers a in task_Waiting[i].coaMembers)
                            {
                                coaMemberList.Add(a);
                            }
                            foreach (Agent ag in agentAll)
                            {
                                foreach (CoaMembers m in task_Waiting[i].coaMembers)
                                {
                                    if (m.agent.ID == ag.ID)
                                    {
                                        ag.statusInCoalition = agentStatusInCoalition.CoalitionAgent;
                                        ag.taskTaken = task_Waiting[i];
                                        m.agent.statusInCoalition = agentStatusInCoalition.CoalitionAgent;
                                    }
                                }
                            }
                            //计算成员到达时间和任务完成时间    
                            task_Waiting[i].finishTime = task_Waiting[i].TaskCompletedTime(ref coaMemberList);
                            task_UnderGoing.Add(task_Waiting[i]);
                        }
                    }
                    //当前任务没有完成联盟优化，则进行如下处理
                    else
                    {
                        foreach (Agent a in agentAll)
                        {
                            if (a.statusInCoalition == agentStatusInCoalition.WaitingAgent)
                            {
                                if (a.taskTaken == null)
                                    a.statusInCoalition = agentStatusInCoalition.IdleAgent;
                                else if (a.taskTaken.potential_leader == a)
                                    a.statusInCoalition = agentStatusInCoalition.IdleAgent;
                            }
                        }
                        //网络重构flag置为true
                        //只要发生一次任务未完成情况，则进行
                        flagAdaption = true;
                    }
                }
                //如果没完成联盟组建
                else
                {
                    foreach (Agent a in agentAll)
                    {
                        if (a.statusInCoalition == agentStatusInCoalition.WaitingAgent)
                        {
                            if (a.taskTaken == null)
                                a.statusInCoalition = agentStatusInCoalition.IdleAgent;
                            else if (a.taskTaken.potential_leader == a)
                                a.statusInCoalition = agentStatusInCoalition.IdleAgent;
                        }
                    }
                    //网络重构flag置为true
                    //只要发生一次任务未完成情况，则进行
                    flagAdaption = true;
                }
            }
            //当发生过有任务未完成情况时
            //进行了网络重构
            #region 网络重构
            switch (flagAdaption)
            {
                case true:
                    foreach (Agent a in agentAll)
                    {
                        if (a.statusInCoalition == agentStatusInCoalition.IdleAgent)
                        {
                            a.AdaptionInNetStructure(agentAll, center);
                        }
                    }
                    break;
                default:
                    break;
            }
            #endregion
            for (int i = 0; i < task_UnderGoing.Count; i++)
            {
                if (task_Waiting.Contains(task_UnderGoing[i]))
                {
                    task_Waiting.Remove(task_UnderGoing[i]);
                }
            }           
            UpdateOfAllTask(stepForShow);
            OutPut(stepForShow);
            UpdateOfAllAgent(stepForShow);                         
        }
        
        /// <summary>
        /// 更新每个智能体的邻居节点以及其位置状态
        /// </summary>
        /// <param name="step">时间节奏</param>
        private void UpdateOfAllAgent(int step)
        {
            for (int i = 0; i < agentAll.Count; i++)
            {
                if (agentAll[i].ID == 48)
                    ;
                //对过期没响应的offer进行清理
                agentAll[i].OfferOrginize(step);
                //对智能体位置状态进行更新                
                agentAll[i].UpDateAgentLocation(step);
                //对智能体的邻域节点网络进行更新
                agentAll[i].NeighborhoodCoalitionConstruct(agentAll, ref agentAll[i].list_Neighbors);
            }

            // 对于智能体的网络值的数据一致性进行更新
            for (int i = 0; i < agentAll.Count; i++)
            {
                foreach (Neighbor nb in agentAll[i].list_Neighbors)
                {
                    foreach (Agent agent in agentAll)
                    {
                        if (nb.agent.ID == agent.ID)
                        {
                            nb.valueAsNeighbor = agent.agentNetValue;
                        }
                    }
                }
            }
        }   
        /// <summary>
        /// 对任务按照当前状态进行分类
        /// </summary>
        /// <param name="step">时间步长</param>
        private void Task_Cateloge(int step)
        {
            //将到达执行时刻的任务放入执行队列
            List<Task> taskList = new List<Task>(0);
            for (int i = 0; i < task_Pending.Count; i++)
            {
                Task ta = new Task();
                if (task_Pending[i].call_time == step)
                {
                    if (task_Pending[i].ID == 11)
                    {
                        ;
                    }
                    ta = task_Pending[i];
                    //对待执行的任务进行智能体预分配
                    //找到负责的agent，并将其纳入联盟中
                    ///同时形成备选智能体集合
                    task_Pending[i].CoalitionPreForm(ref agentAll);
                    task_Pending[i].status = Task.taskStatus.waitingCoalition;
                    task_Waiting.Add(task_Pending[i]);
                    taskList.Add(ta);
                }
                if (task_Pending[i].deadline < step)
                {
                    ta = task_Pending[i];
                    task_Waiting.Add(task_Pending[i]);
                    taskList.Add(ta);
                }
            } 
            //将执行的任务移出pending队列
            for (int i = 0; i < taskList.Count; i++)
            {
                task_Pending.Remove(taskList[i]);
            }
            taskList.Clear();
            for (int i = 0; i < task_Waiting.Count; i++)
            {
                if (task_Waiting[i].deadline < step)
                {
                    //task_waiting[i].arrive_time = task_waiting[i].arrive_time + step;   
                    //当超过时限时，则将任务移入不能完成任务队列     
                    task_UnComplete.Add(task_Waiting[i]);
                    taskList.Add(task_Waiting[i]);
                }
            }
            foreach (Task task in taskList)
            {
                for (int i = 0; i < task_Waiting.Count; i++)
                {
                    if (task.ID == task_Waiting[i].ID)
                    {
                       
                        //找到对应的任务ID，对其进行waiting消除处理
                        task_Waiting.Remove(task);
                    }
                }
            }
            taskList.Clear();
        }
      
        /// <summary>
        /// 对任务状态进行更新
        /// </summary>
        /// <param name="step">当前时刻</param>
        private void UpdateOfAllTask(int step)
        {
            List<Task> taskList = new List<Task>(0);
            List<CoaMembers> coaMemberList = new List<CoaMembers>(0);
           foreach(Agent a in agentAll)
            {
                CoaMembers m = new CoaMembers();
                m.agent = a;
                m.rank = 0;
                m.valueOfAgent = 0;
                coaMemberList.Add(m);
            }
            for (int i = 0; i < task_UnderGoing.Count; i++)
            {
                //if (task_UnderGoing[i].IsTaskCompleted(step,ref coaMemberList))
                if (step >= task_UnderGoing[i].finishTime)
                {
                    task_UnderGoing[i].status = Task.taskStatus.completed;
                    task_Complete.Add(task_UnderGoing[i]);
                    taskList.Add(task_UnderGoing[i]);
                }
            }
            for (int i = 0; i < taskList.Count; i++)
            {
                if (task_UnderGoing.Contains(taskList[i]))
                {
                    task_UnderGoing.Remove(taskList[i]);
                }
            }
            foreach (CoaMembers m in coaMemberList)
            {
                for(int i =0; i< agentAll.Count;i++)
                {
                    if (agentAll[i].ID == m.agent.ID)
                    {
                        agentAll[i] = m.agent;
                    }
                }
            }
        }     

        /// <summary>
        /// 即时输出执行任务的agent的坐标变化
        /// </summary>
        private void OutPut(int step)
        {
            string AgentStream = "";
            string TaskStream = "";
            for (int i = 0; i < agentAll.Count; i++)
            {
                //将当前时刻的任务分配情况进行发布
                if (agentAll != null && agentAll.Count != 0 && (agentAll[i].statusInCoalition == agentStatusInCoalition.CoalitionAgent))/*|| agentAll[i].statusInCoalition == agentStatusInCoalition.AdaptionInNetAgent))*/
                {
                    AgentStream = "\n time " + step.ToString() +",The agent stutes: " +agentAll[i].statusInCoalition.ToString()+ "\n" + agentAll[i].ID.ToString() + "its current location  x: " 
                        + agentAll[i].curtLocattion.lat.ToString() + ",   y:" + agentAll[i].curtLocattion.lot.ToString() + "\n";
                    AgentInfo += AgentStream;
                    Console.WriteLine(AgentStream);
                    if (agentAll[i].taskTaken != null)
                    {
                        AgentStream = "the task ID is" + agentAll[i].taskTaken.ID.ToString() + "\n";
                    }
                    Console.WriteLine(AgentStream);
                    AgentInfo += AgentStream;
                }
            }
            for (int i = 0; i < task_Complete.Count; i++)
            {
                if (task_Complete[i].status == Task.taskStatus.completed)
                {
                    TaskStream = "Time " + step.ToString() + ", completed task ID" + task_Complete[i].ID.ToString() + ": , its location is  x " + task_Complete[i].lat.ToString() + ", y " + task_Complete[i].lot.ToString() +"\n";
                    float x, y;
                    x = y = 0;
                    foreach (CoaMembers a in task_Complete[i].coaMembers)
                    {
                        TaskStream += ",ID " + a.agent.ID.ToString();
                        x += a.agent.abilityA.ablityX;
                        y += a.agent.abilityA.ablityY;
                        //任务完成，对任务执行agent进行释放
                        foreach (Agent agentrelease in agentAll)
                        {
                            if (agentrelease.ID == a.agent.ID)
                            {
                                agentrelease.statusInCoalition = agentStatusInCoalition.IdleAgent;
                                agentrelease.taskTaken = new Task();
                            }
                        }
                    }
                    TaskStream += "\n requirement, x= " + task_Complete[i].requirement.ablityX.ToString() + ",y = " + task_Complete[i].requirement.ablityY.ToString();
                    TaskStream += "\n actually, x= " + x.ToString() + ",y =" + y.ToString();
                    distance.ablityY += task_Complete[i].requirement.ablityY;
                    distance.ablityX += task_Complete[i].requirement.ablityX;
                    totalAblity.ablityX += x;
                    totalAblity.ablityY += y;
                    TaskStream += "\n gap  x =,  " + distance.ablityX.ToString() + ", y =," + distance.ablityY.ToString();
                    TaskStream += "\n total ability x =,  " + totalAblity.ablityX.ToString() + ", y =," + totalAblity.ablityY.ToString();
                    TaskStream += "\n" + task_Complete.Count.ToString() + "\n";
                    TaskStream += "\n unresponded tasks: " + task_UnComplete.Count.ToString() + "\n";
                    Console.WriteLine(TaskStream);
                    TaskInfo += TaskStream;
                    task_Complete[i].status = Task.taskStatus.haldled;
                }               
            }
        }
    }
}
