using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFG
{
    public class ablity
    {
        public float ablityX;
        public float ablityY;
    }

    /// <summary>
    /// 备选智能体
    /// </summary>
    public class CoaMembers
    {
        /// <summary>
        /// 智能体
        /// </summary>
        public Agent agent;
        /// <summary>
        /// 智能体在联盟中贡献值，由能力贡献和成本共同组成
        /// </summary>
        public float valueOfAgent;
        /// <summary>
        /// 在备选智能体中的排序
        /// </summary>
        public int rank;
    }

    public class Task
    {
        /// <summary>
        /// 任务的状态
        /// </summary>
        public enum taskStatus
        {
            waitingCoalition,
            waitingLeader,
            idle,
            haldled,
            /// <summary>
            /// 已经有联盟
            /// </summary>
            assignedWithoutStart, 
            started,
            uncompleted, //不能完成
            completed,
        }
        
       
        /// <summary>
        /// 任务位置
        /// </summary>
        public float lot;
        public float lat;
        /// <summary>
        /// 任务的申报时间
        /// </summary>
        public int call_time;
        /// <summary>
        /// 联盟到达时间
        /// </summary>
        public int arrive_time;
        /// <summary>
        /// 联盟形成的截止时间
        /// </summary>
        public int deadline;
        /// <summary>
        /// 任务完成需要的时间
        /// </summary>
        public int period_time;
        /// <summary>
        /// 任务完成计时器
        /// </summary>
        public int excuting_counter;
        /// <summary>
        ///任务的回报值
        /// </summary>
        public int reward;
        /// <summary>
        /// 任务能被感知的范围
        /// </summary>
        private float x;
        /// <summary>
        /// 任务需求，二元要求
        /// </summary>
        public ablity requirement;
        /// <summary>
        /// 待联盟智能体成员,用于储备备选智能体
        /// </summary>
        public List<CoaMembers> pre_members;   
        /// <summary>
        /// 联盟领导
        /// </summary>
        public Agent potential_leader;
        /// <summary>
        /// 能被发现的任务距离
        /// </summary>
        public float scanLength;
        /// <summary>
        /// 任务状态
        /// </summary>
        public taskStatus status;
        /// <summary>
        /// 任务序号，用于区别
        /// </summary>
        public int ID;
        /// <summary>
        /// 开始进行联盟构建时间
        /// </summary>
        public int start_formationTime;
        /// <summary>
        /// 确定的联盟成员
        /// </summary>
        public List<CoaMembers> coaMembers;
        /// <summary>
        /// 联盟生成时间
        /// </summary>
        public float startTime;
        /// <summary>
        /// 任务完成时间
        /// </summary>
        public int finishTime;
        /// <summary>
        /// 信息发布者智能体选择
        /// 对每一个任务，将其附近空间距离最近的智能体作为其coalition最开始的集合
        /// 模拟任务发现发布过程的函数
        /// </summary>
        /// <param name="aList">全体智能体</param>
        public void CoalitionPreForm(ref List<Agent> aList)
        {
            ///探测范围在150之内的空闲agent都进行联盟结构搜索
            float f = 0;
            float dis = 0;
            int mark =0;
            dis = aList[0].DistComp(this);
            //选择离任务最近的智能体作为信息获取者
            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i].statusInCoalition == agentStatusInCoalition.IdleAgent && aList[i].list_Neighbors!=null)
                {
                    f = aList[i].DistComp(this);
                    if (f < dis)
                    {
                        //如果第一个被通知的agent的邻居节点不为空，那么在它的邻居节点中进行coalition组建的消息传播
                        mark = i;
                        dis = f;
                    }                 
                }
            }
            //选择离任务最近的智能体作为信息获取者
            if (mark!=0 || dis !=0)
            {
                potential_leader = aList[mark];
                aList[mark].statusInCoalition = agentStatusInCoalition.WaitingAgent;
                aList[mark].taskTaken = this;
                CoaMembers m = new CoaMembers();
                m.agent = aList[mark];
                m.rank = 0;
                m.valueOfAgent = 0;
                pre_members.Add(m);
            }
            //
            else if(mark == 0 && dis==0)
            {
               do
                {
                    Random ra = new Random();
                    int j = ra.Next(aList.Count);
                    if (aList[j].statusInCoalition == agentStatusInCoalition.IdleAgent && aList[j].list_Neighbors !=null && j>=0 && j< aList.Count)
                    {
                        potential_leader = aList[j];
                        aList[j].statusInCoalition = agentStatusInCoalition.WaitingAgent;
                        aList[j].taskTaken = this;
                        CoaMembers m = new CoaMembers();
                        m.agent = aList[j];
                        m.rank = 0;
                        m.valueOfAgent = 0;
                        pre_members.Add(m);
                        break;
                    }
                }while(potential_leader == null);                    
            }
        }

        /// <summary>
        /// 判断任务是否完成，将任务设置为一个需时为固定时间的任务，而执行任务时间为联盟内部到达最晚成员时间
        /// </summary>
        /// <returns></returns>
        public int TaskCompletedTime(ref List<CoaMembers> aList)
        {
            float time = 0;
            int mark;
            if (CoaAbilityCheck(aList))  //如果完成联盟构建,计算到达最晚的一个
            {
                float timeF = 0;
                for (int i = 0; i < coaMembers.Count; i++)
                {
                    timeF = coaMembers[i].agent.DistComp(this) / coaMembers[i].agent.speed;
                    if (timeF > time)
                    {
                        time = timeF;
                        mark = i;
                    }
                }
                return (int)(time + period_time + startTime);
            }
                //不满足任务要求则不能完成任务，将r任务完成时间设置成零
            else
                return 0;
        }

        /// <summary>
        /// 判断任务是否完成
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool IsTaskCompleted(int step,ref List<CoaMembers> aList)
        {
            int finishTime = TaskCompletedTime(ref aList);
            if (finishTime != 0 && CoaAbilityCheck(coaMembers) && (step >= finishTime))
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 计算联盟是否完成构建,构建成功则返回true，否则返回false
        /// </summary>
        /// <returns></returns>
        public bool CoaAbilityCheck(List<CoaMembers> aList)
        {
            float x, y;
            x = y = 0;
            for (int i = 0; i < aList.Count; i++)
            {
                x += aList[i].agent.abilityA.ablityX;
                y += aList[i].agent.abilityA.ablityY;
            }
            
            if (x >= requirement.ablityX && y >= requirement.ablityY)
            {
                return true;
            }   
        return false;
        }

        /// <summary>
        /// 能力差距计算函数
        /// </summary>
        /// <returns>返回能力差距</returns>
        public ablity coaRequireCheck(List<CoaMembers> coaGroup)
        {
            float x, y;
            x = y = 0;
            ablity a;
            a = new ablity();

            for (int i = 0; i < coaGroup.Count; i++)
            {
                x += coaGroup[i].agent.abilityA.ablityX;
                y += coaGroup[i].agent.abilityA.ablityY;
            }
            if (x < requirement.ablityX || y < requirement.ablityY)
            {
                a.ablityX = requirement.ablityX - x;
                a.ablityY = requirement.ablityY - y;
            }
            else
            {
                a.ablityX = 0;
                a.ablityY = 0;
            }
            return a;
        }

        /// <summary>
        /// 多余智能体的分割
        /// </summary>
        /// <param name="aCoa">需要进行分割的联盟</param>
        public CoaMembers Split(List<CoaMembers> aCoa)
        {
            int number =0;
            CoaMembers agent = new CoaMembers();
            CoaMembers agentNew = new CoaMembers();
            float value,valueOri;
            float valueIni =value= valueOfCoalition(aCoa);
            valueOri = value;
            agent = aCoa[0];
            aCoa.Remove(agent);
            valueIni = valueOri - valueOfCoalition(aCoa);
            aCoa.Insert(0,agent);
            for (int i = 1; i < aCoa.Count; i++)
            {
                agent = aCoa[i];
                aCoa.Remove(agent);
                value = valueOri - valueOfCoalition(aCoa);
                if (value > valueIni)
                {
                    valueIni = value;
                    number = i;
                    agentNew = aCoa[i];
                }
                aCoa.Insert(i,agent);
            }
            return agentNew;
        }

        /// <summary>
        /// 联盟组队优化选择
        /// 考虑移动成本和到达时间进行选择
        /// </summary>
        public bool CoalitionOptimization(Task task,int step)
        {
            bool flag = false;
            CoaMembers newCoalitionAgent = new CoaMembers();
            coaMembers = new List<CoaMembers>(0);
            //当备选智能体
            if (CoaAbilityCheck(task.pre_members))
            {
                do
                {
                    //获取新添加的智能体的ID
                    newCoalitionAgent = AddAgent(coaMembers, task.pre_members);
                    if (newCoalitionAgent == null )
                    {
                        return false;
                    }
                    if (newCoalitionAgent.agent == null)
                    {
                        return false;
                    }
                    if (!IsAgentInCoalition(coaMembers,newCoalitionAgent.agent))
                    {
                        newCoalitionAgent.agent.taskTaken = task;
                        coaMembers.Add(newCoalitionAgent);
                        pre_members.Remove(newCoalitionAgent);
                        flag = CoaAbilityCheck(coaMembers);
                        //如果联盟能力超过了要求，则进行联盟分解
                        if (!flag)
                        {
                            ablity ab = coaRequireCheck(coaMembers);
                            if (pre_members.Count != 0 && (ab.ablityX > requirement.ablityX || ab.ablityY > requirement.ablityY))
                            {
                                newCoalitionAgent = Split(coaMembers);
                                coaMembers.Remove(newCoalitionAgent);
                            }
                            //else
                            //    start_time = step + period_time;
                        }
                    }
                } while (!flag);
            }
            else
                call_time = step + 1;
            return true;
        }


        /// <summary>
        /// 对智能体a1能否在联盟内进行判断，在则返回true，不在则返回false
        /// </summary>
        /// <param name="coalitionList"></param>
        /// <param name="a1">被判断智能体</param>
        /// <returns></returns>
        private bool IsAgentInCoalition(List<CoaMembers> coalitionList,Agent a1)
        {
            if (coalitionList != null && coalitionList.Count != 0)
            {
                if (a1 != null)
                {
                    foreach (CoaMembers m in coalitionList)
                    {
                        if (m.agent.ID == a1.ID)
                            return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 从备选智能体中选择下一个智能体，选择标准是最大化当前联盟值
        /// </summary>
        /// <param name="PreCoaMemList">备选智能体集合</param>
        /// <returns>返回被选中参加联盟的智能体</returns>
        private CoaMembers AddAgent(List<CoaMembers> PreCoaMemList, List<CoaMembers> reservedAgents)
        {
            if (reservedAgents.Count == 0)
                return null;
            CoaMembers newCm = new CoaMembers();
            float valueOfCoa, valueIni;
            valueIni = 0;
            if (PreCoaMemList != null && PreCoaMemList.Count != 0)
            {
                PreCoaMemList.Add(reservedAgents[0]);
                valueIni = valueOfCoalition(PreCoaMemList);
                newCm = reservedAgents[0];
                PreCoaMemList.Remove(reservedAgents[0]);
            }           
            for(int i = 1;i< reservedAgents.Count;i++)
            {
                if (!IsAgentInCoalition(PreCoaMemList,reservedAgents[i].agent))
                {
                    PreCoaMemList.Add(reservedAgents[i]);
                    valueOfCoa = valueOfCoalition(PreCoaMemList);
                    if (valueIni < valueOfCoa)
                    {
                        valueIni = valueOfCoa;
                        newCm = reservedAgents[i];
                    }
                    PreCoaMemList.Remove(reservedAgents[i]);
                }
            }
            return newCm;
        }



        /// <summary>
        /// 进行coalition需要调整的成员选择
        /// </summary>
        /// <param name="CoaList"></param>
        /// <returns></returns>
        private int DeleteAgent(List<CoaMembers> CoaList)
        {
            float value = 0;
            value = valueOfCoalition(CoaList);
            return 1;
        }
        /// <summary>
        /// 联盟值计算
        /// </summary>
        /// <param name="CoaList">联盟C的值</param>
        /// <returns></returns>
        private float valueOfCoalition(List<CoaMembers> CoaList)
        {
            //联盟值
            float valueOfCoalition = 0;
            //能力值
            float valueOfAbility = 0;
            if (CoaList.Count != 0)
            {
                double LateTime = 0; //联盟中最晚到达的成员的到达时间
                LateTime = CoaList[0].agent.DistComp(this) / CoaList[0].agent.speed;
                foreach (CoaMembers a in CoaList)
                {
                    double time = a.agent.DistComp(this) / a.agent.speed;
                    if (time > LateTime)
                    {
                        LateTime = time;
                    }
                }               
                float x, y;
                x = y = 0;
                for (int i = 0; i < CoaList.Count; i++)
                {
                    x = CoaList[i].agent.abilityA.ablityX;
                    y = CoaList[i].agent.abilityA.ablityY;
                    valueOfAbility = (float)(Math.Abs(1 - (0.5 * x + 0.5 * y) / (0.5 * requirement.ablityX + 0.5 * requirement.ablityY)));
                    valueOfCoalition += (float)Math.Pow((double)(valueOfAbility), (LateTime));
                }
                // 对联盟C进行值计算   
                //LateTime = 0 - LateTime;
               
                double f = Math.Pow((double)reward, LateTime);
                //valueOfAbility = (float)(valueOfAbility);
            }
            return valueOfCoalition;
        }
    }
}
