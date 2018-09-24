using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFG
{
    /// <summary>
    /// 智能体对于联盟的状态
    /// </summary>
    public enum agentStatusInCoalition
    {
        /// <summary>
        /// 等待联盟的状态
        /// </summary>
        IdleAgent = 0,
        /// <summary>
        /// 已经完成分配的agent
        /// </summary>
        CoalitionAgent = 1,
        /// <summary>
        /// 正在协商组建coalition的agent
        /// </summary>
        WaitingAgent = 2,
        /// <summary>
        /// 网络自适应协调中的agent
        /// </summary>
        AdaptionInNetAgent = 3,
        /// <summary>
        /// 执行任务的智能体
        /// </summary>
        ExecutingAgent =4,
    }   

    /// <summary>
    /// 智能体所处的网络状态
    /// </summary>
    public enum LineType
   {
        /// <summary>
        /// 普通斜率直线
        /// </summary>
        NormalLine = 0,
        /// <summary>
        ///水平线
        /// </summary>
        HorizontalLine = 1,
        /// <summary>
        /// 垂直线
        /// </summary>
        VerticalLine =2,
        /// <summary>
        /// 到达目标点
        /// </summary>
        SamePt =3,
    }

    /// <summary>
    /// 智能体所在位置的智能体密度
    /// </summary>
    public enum NetStatus
    {
        /// <summary>
        /// 稀疏网络状态，当网络邻居节点少于三个时
        /// </summary>
        Sparse_Network= 1,
        /// <summary>
        /// 拥挤网络状态，当网络邻居节点多于十个时（由于有上限要求，因此并没有真正有十个邻居，但是处于一个非常密集的智能体网络中）
        /// </summary>
        Crowded_Network =2,
    }

    /// <summary>
    /// 用于描述策略库，记录对应的数据，方便计算
    /// </summary>
    public struct Strategy
    {
        /// <summary>
        /// 备选策略
        /// </summary>
        public Neighbor item_Neighbor;
        /// <summary>
        /// 当前策略对应的最佳地理位置
        /// </summary>
        public Location locationOpt;
        /// <summary>
        /// 置信度
        /// </summary>
        public float belief;
        /// <summary>
        /// 选择的策略成本
        /// </summary>
        public float cost;
        /// <summary>
        /// 选择后获得的收益
        /// </summary>
        public float welfare;
        /// <summary>
        /// 标志位，用于标注当前策略是不是策略集中最高收益
        /// </summary>
        public bool flag;
    }

    public class agentUtilityInLocalNet
    {
        /// <summary>
        /// 邻居节点在本节点网络中的贡献
        /// </summary>
        public float ContributionInNet;
        /// <summary>
        /// 对本节点的能力值提升贡献
        /// </summary>
        public float ContributionOfAbility;
        /// <summary>
        /// 所能带来的次邻居节点资源
        /// </summary>
        public float ContributionInSubN;
    }

    /// <summary>
    /// 邻居节点的定义，用于本地存储邻居节点的一些必要信息，以减少内存消耗。需要对邻居节点进行信息一致性维护
    /// </summary>
    public class Neighbor
    {
        /// <summary>
        /// 邻居节点
        /// </summary>
        public Agent agent;
        /// <summary>
        /// 每个邻居节点对当前智能体的贡献的网络utility
        /// </summary>
        public agentUtilityInLocalNet valueAsNeighbor;
        /// <summary>
        /// 每个次邻居节点对当前智能体网络贡献的utility
        /// </summary>
        public agentUtilityInLocalNet valueAsSubNeighbor;
        /// <summary>
        /// 本智能体对每个邻居节点会离开当前联盟的置信度
        /// 暂时不考虑，将在以后的计算中对算法进行进一步改进时再进行利用
        /// </summary>
        public float belief;
        /// <summary>
        /// 与当前节点邻居集重叠数量
        /// </summary>
        public int overlappingNumInN;
        /// <summary>
        /// 次邻居节点与当前邻居节点以及次邻居节点的重叠数量
        /// </summary>
        public int overlappingNumInSN;
        /// <summary>
        /// 失去当前智能体之后的utility变化率
        /// </summary>
        public float utilityCP;
    }
    /// <summary>
    /// 用于定义智能体目标区域
    /// </summary>
    public class InterSection
    {
        /// <summary>
        /// 最大纬度
        /// </summary>
        public float LatMax;
        /// <summary>
        /// 最小纬度
        /// </summary>
        public float LatMin;
        /// <summary>
        /// 最大经度
        /// </summary>
        public float LotMax;
        /// <summary>
        /// 最小经度
        /// </summary>
        public float LotMin;
    }
    /// <summary>
    /// 排序指示类
    /// </summary>
    public class ContributionComper : IComparer<Neighbor>
    {
        public int Compare(Neighbor n1, Neighbor n2)
        {
            return (n2.valueAsNeighbor.ContributionOfAbility.CompareTo(n1.valueAsNeighbor.ContributionOfAbility));
        }
    }

    /// <summary>
    /// 合约类
    /// </summary>
    public class Offers
    {
        /// <summary>
        /// offer的提供者
        /// </summary>
        public Agent leader;
        /// <summary>
        /// offer的要求
        /// </summary>
        public double cost;
        /// <summary>
        /// 提供给agent的收益
        /// </summary>
        public float pay;
        /// <summary>
        /// 是否参与
        /// </summary>
        public bool resp;
        /// <summary>
        /// 定义offer转发次数
        /// </summary>
        public int PassTime;
        /// <summary>
        /// 关于合约的任务
        /// </summary>
        public Task task;
        /// <summary>
        /// 回应智能体预计到达时间
        /// </summary>
        public float aTofAgent;
        /// <summary>
        /// 合约有效时间
        /// </summary>
        public int offerT;
    }

    public struct Location
    {
        /// <summary>
        /// 经度
        /// </summary>
        public float lat;
        /// <summary>
        /// 纬度
        /// </summary>
        public float lot;
    }

    /// <summary>
    /// 当前智能体本身的DC属性
    /// </summary>
    public struct CNAMe
    {
        /// <summary>
        /// 当前智能体的Degree centrality
        /// </summary>
        public float DC;
        /// <summary>
        /// 当前节点本身的权重DC值
        /// </summary>
        public float wDC;
        /// <summary>
        /// 当前节点的Local
        /// </summary>
        public float LocalR;
    }

    /// <summary>
    /// 当前智能体所在局域网络的参数指标
    /// </summary>
    public struct ComplexNetAttrLocal
    {        
        /// <summary>
        /// 当前智能体所在局域网的DC值
        /// </summary>
        public float LDC;
        /// <summary>
        /// 当前智能体所在局域网的权重DC值
        /// </summary>
        public float wLDC;      
        /// <summary>
        /// 节点所在局域网结构的熵
        /// </summary>
        public float eIs;
        /// <summary>
        /// 节点所在局域网能力的熵
        /// </summary>
        public float eIa;
        /// <summary>
        /// 当前节点的Local值，用于后续计算
        /// </summary>
        public int localR;
    }
    /// <summary>
    /// 智能体类
    /// </summary>
    public class Agent
    {
        /// <summary>
        /// 智能体当前位置
        /// </summary>
        public Location curtLocattion;
        /// <summary>
        /// 个体盈利，用于决策判断
        /// </summary>
        public float profit;
        /// <summary>
        /// 用于描述智能体有没有自由来进行决策
        /// </summary>
        public float keyPointValue;
        /// <summary>
        /// 智能体当前状态
        ///</summary> 
        public agentStatusInCoalition statusInCoalition;
        /// <summary>
        /// 邻居节点的数量上限是10
        /// </summary>
        private int maxNeighborCount = 10;
        /// <summary>
        /// 能互相通信的智能体邻居，作为一个公共的通信池，需保持数据的一致性
        /// </summary>
        public List<Neighbor> list_Neighbors;
        /// <summary>
        /// 次邻居节点集合,用于选择最佳的coalition以进行自适应
        /// </summary>
        public List<Neighbor> list_subNeighbors;
        /// <summary>
        /// 每分钟油耗
        /// </summary>
        public float fuelCost;
        /// <summary>
        /// 每秒飞行速度
        /// </summary>
        public float speed;
        /// <summary>
        /// 智能体对于接受了的offer的忠诚度
        /// </summary>
        private float loyalty;
        /// <summary>
        /// 每个智能体的编号
        /// </summary>
        public int ID;
        /// <summary>
        /// 智能体收到的联盟邀约
        /// </summary>
        public List<Offers> potential_Offers;
        /// <summary>
        /// 经济成本
        /// </summary>
        //private float price;
        /// <summary>
        /// 智能体的能力
        /// </summary>
        public ablity abilityA;
        /// <summary>
        /// 承接的任务
        /// </summary>
        public Task taskTaken;
        /// <summary>
        /// 通信半径
        /// </summary>
        public float comRadio;
        /// <summary>
        /// 巡逻速度
        /// </summary>
        public float cruiseSpeed;
        /// <summary>
        /// 智能体的个体价值
        /// </summary>
        public agentUtilityInLocalNet agentNetValue;
        /// <summary>
        /// 策略空间
        /// </summary>
        public List<Strategy> strategy_List;
        /// <summary>
        /// 当前节点本身的DC属性
        /// </summary>
        public CNAMe cnaM;
        /// <summary>
        /// 复杂网络中的属性
        /// </summary>
        public ComplexNetAttrLocal LocalCNA;
        /// <summary>
        /// 在复杂网络中的度
        /// </summary>
        public int DegreeOfComplex;
        /// <summary>
        /// 每个智能体的自由度，邻居节点越少自由度越高
        /// </summary>
        public float freedom;
        /// <summary>
        /// 历史中离开当前位置的比率
        /// </summary>
        public float pTrue;
        /// <summary>
        /// 决策过的总次数
        /// </summary>
        private int count = 0;
        /// <summary>
        /// 位置移动的历史次数
        /// </summary>
        private int leaveCount = 0;
        /// <summary>
        /// 策略选择
        /// </summary>
        private bool chooseFlag = false;
        /// <summary>
        /// 用于存放新位置
        /// </summary>
        public Location destLoc = new Location();
        /// <summary>
        /// 被选中参与三角形外接圆计算
        /// </summary>
        public int choseCount = 0;
        /// <summary>
        /// 初始化所处网络为稀疏网络
        /// </summary>
        private NetStatus netStatus = NetStatus.Sparse_Network;
        /// <summary>
        /// 预估的到达任务点时间
        /// </summary>
        public int arriveTime;
        /// <summary>
        /// 拥挤网络空间调整量
        /// </summary>
        private int spaceAdaption = 10;
        /// <summary>
        /// 在网络中位置稳定的概率
        /// </summary>
        public float stableProbInNet = 0;
 
        #region 智能体方法    

        /// <summary>
        /// 智能体a的Neighborhood的utility的计算函数
        /// </summary>
        /// <param name="a1">智能体A</param>
        /// <returns></returns>
        public agentUtilityInLocalNet valueOfNeighborhoodCoalition(Agent a1) //计算个体的utility on resource
        {
            agentUtilityInLocalNet aNV = new agentUtilityInLocalNet();
            aNV.ContributionInNet = 0;
            aNV.ContributionOfAbility = 0;
            aNV.ContributionInSubN = 0;
            float Nr = 0;
            Nr = (float)(0.5 * a1.abilityA.ablityX + 0.5 * a1.abilityA.ablityY);
            agentNetValue.ContributionOfAbility = Nr;
            aNV.ContributionOfAbility = Nr;
            float Nn = 0;
            //计算邻居节点能带来的local net的utility（包括次邻居节点）
            if (a1.list_Neighbors != null && a1.list_Neighbors.Count != 0)
            {
                foreach (Neighbor nb in a1.list_Neighbors)
                {
                    Nr = (float)(0.5 * nb.agent.abilityA.ablityX + 0.5 * nb.agent.abilityA.ablityY); 
                    Nn += Nr;
                    foreach (Neighbor nb2 in nb.agent.list_Neighbors)
                    {
                        //对于次邻居节点nb2，对于a的邻居集合的utility贡献
                        if (!IsNbInNeighbors(nb2, a1))
                        {
                                aNV.ContributionInSubN += (float)(0.5 * nb2.agent.abilityA.ablityX + 0.5 * nb2.agent.abilityA.ablityY); 
                        }
                    }
                }
                //计算次邻居节点集的utility
                //foreach (Neighbor nb1 in listNeighbor)
                //{
                //    ;
                //}
                aNV.ContributionInNet = Nn;             
            }
            return aNV;
        }

        /// <summary>
        /// 智能体a的Neighborhood的utility的计算函数
        /// </summary>
        /// <param name="a1">智能体A</param>
        /// <param name="list_agent">全体智能体</param>
        /// <param name="temp_Neighbors">智能体a1临时的邻居节点集</param>
        /// <returns></returns>
        private agentUtilityInLocalNet valueOfHNeighborhoodCoalition(Agent a1, List<Agent> list_agent,List<Neighbor> temp_Neighbors) //计算个体的utility on resource
        {
            agentUtilityInLocalNet aNV = new agentUtilityInLocalNet();
            aNV.ContributionInNet = 0;
            aNV.ContributionOfAbility = 0;
            aNV.ContributionInSubN = 0;
            float Nr = 0;
            Nr = (float)(0.5 * a1.abilityA.ablityX + 0.5 * a1.abilityA.ablityY);
            agentNetValue.ContributionOfAbility = Nr;
            aNV.ContributionOfAbility = Nr;
            float Nn = 0;
            //计算邻居节点能带来的local net的utility（包括次邻居节点）
            if (temp_Neighbors != null && temp_Neighbors.Count != 0)
            {
                foreach (Neighbor nb in temp_Neighbors)
                {
                    Nr = (float)(0.5 * nb.agent.abilityA.ablityX + 0.5 * nb.agent.abilityA.ablityY);
                    Nn += Nr;
                    foreach (Neighbor nb2 in nb.agent.list_Neighbors)
                    {
                        //对于次邻居节点nb2，对于a的邻居集合的utility贡献
                        if (!IsNbInNeighbors(nb2, a1))
                        {
                            aNV.ContributionInSubN += (float)(0.5 * nb2.agent.abilityA.ablityX + 0.5 * nb2.agent.abilityA.ablityY);
                        }
                    }
                }               
                aNV.ContributionInNet = Nn;
            }
            return aNV;
        }

        /// <summary>
        /// 智能体a1作为当前智能体次邻居节点之后的utilityd的计算函数
        /// </summary>
        /// <param name="a1">目标智能体</param>
        /// <param name="list_agent">全体智能体集合</param>
        /// <returns></returns>
        private agentUtilityInLocalNet valueOfAgentAsNewNeighbor(Agent a1, List<Agent> list_agent) //计算个体的utility on resource
        {
            agentUtilityInLocalNet aNV = new agentUtilityInLocalNet();
            aNV.ContributionInNet = 0;
            aNV.ContributionOfAbility = 0;
            List<Neighbor> listNeighborTemp = new List<Neighbor>(0);
            //对a1的邻居节点进行更新
            a1.NeighborhoodCoalitionConstruct(list_agent, ref a1.list_Neighbors);
            //使用一个临时列表来进行邻居节点保存，避免出现指针混乱问题
            foreach (Neighbor nb in this.list_Neighbors)
            {
                Neighbor nb1 = new Neighbor();
                nb1 = nb;
                listNeighborTemp.Add(nb1);
            }
            Neighbor nb2 = new Neighbor();
            //计算智能体a1的NeighhoodCoalition
            a1.agentNetValue = valueOfNeighborhoodCoalition(a1);
            nb2.agent = a1;
            nb2.belief = 1;
            //计算得到本智能体和A1之间的资源重叠度（邻居点的重叠度）
            nb2.overlappingNumInN = OverLappingInNeighbors(a1,this);
            //将智能体A1添加到本智能体的邻居节点集
            listNeighborTemp.Add(nb2);
            //计算a1成为当前智能体节点的邻居节点之后能带来的utility
            //
            //计算邻居节点能带来的local net的utility（包括次邻居节点）
            if (a1.list_Neighbors != null && a1.list_Neighbors.Count != 0)
            {
                foreach (Neighbor nb in a1.list_Neighbors)              
                {
                    if (!IsNbInNeighbors(nb, this))
                    {
                        aNV.ContributionInSubN += (float)(0.5 * nb.agent.abilityA.ablityX + 0.5 * nb.agent.abilityA.ablityY);
                    }
                }
            }
            aNV.ContributionOfAbility = a1.agentNetValue.ContributionOfAbility;
            //次邻居节点的邻居节点的网络资源并不计入，因为对于本智能体来说，第三层邻居节点网络utility无意义
            aNV.ContributionInNet = aNV.ContributionInSubN;          
            return aNV;
        }

        /// <summary>
        /// 用于统计某节点的1次邻居和2次邻居节点的总个数
        /// </summary>
        /// <param name="aList">用于查找邻居节点</param>
        /// <returns></returns>
        private int DifferInNeighbors(List<Agent> aList, List<Neighbor> listNB)
        {
            List<int> nbs = new List<int>(0);
            for (int i = 0; i < listNB.Count; i++)
            {
                nbs.Add(this.ID);
                foreach (Neighbor a in listNB)
                    nbs.Add(a.agent.ID);
                for (int j = 0; j < aList.Count; j++)
                {
                    if (listNB[i].agent.ID == aList[j].ID)
                        foreach (Neighbor a in aList[j].list_Neighbors)
                        {
                            if (!nbs.Contains(a.agent.ID))
                                nbs.Add(aList[j].ID);
                        }
                }
            }
            return nbs.Count;
        }

        /// <summary>
        /// 本智能体的二次邻居节点的集合的计算
        /// </summary>
        /// <returns></returns>
        public void CoalitionOfSubNeighborsConstruct(ref List<Neighbor> SubNeighbors)
        {
            for (int i = 0; i < list_Neighbors.Count; i++)
            {
                foreach (Neighbor a in list_Neighbors[i].agent.list_Neighbors)
                {
                    //如果当前次邻居节点不存在当前智能体的邻居节点中，并且不存在他的次邻居节点集合中，则将该节点添加到
                    //并且当前节点不能是自己的次邻居节点
                    if (!SubNeighbors.Contains(a) && !list_Neighbors.Contains(a) && this.ID != a.agent.ID)
                        SubNeighbors.Add(a);
                }

            }
        }

        /// <summary>
        /// 当前节点关于对邻居节点能力需求的打分
        /// </summary>
        /// <param name="aList">用于查找邻居节点</param>
        /// <returns></returns>
        private float DifferInAbility(List<Agent> aList)
        {
            float x, y, z;
            z = 0.0f;
            for (int i = 0; i < list_Neighbors.Count; i++)
            {
                x = (list_Neighbors[i].agent.abilityA.ablityX - abilityA.ablityX) / abilityA.ablityX;
                y = (list_Neighbors[i].agent.abilityA.ablityY - abilityA.ablityY) / abilityA.ablityY;
                z = (float)(0.5 * x + 0.5 * y);
            }
            return z;
        }

        /// <summary>
        /// 邻居节点对于本节点的local网络贡献值
        /// </summary>
        /// <param name="agent">指定节点对本节点网络拓扑结构的贡献值</param>
        private float ContributionOfPotenmtialNeighbors(Agent agent)
        {
            float value = 0;
            float timeCost; ;
            List<int> nbs = new List<int>(0);
            List<int> sbs = new List<int>(0);
            //本节点的邻居节点数量
            foreach (Neighbor a in list_Neighbors)
            {
                nbs.Add(a.agent.ID);
            }
            //本节点的指定节点的异构节点，与本节点邻居节点相异的节点数
            foreach (Neighbor a in agent.list_Neighbors)
            {
                if (!nbs.Contains(a.agent.ID))
                {
                    sbs.Add(a.agent.ID);
                }
            }
            value = sbs.Count / agent.list_Neighbors.Count;
            timeCost = DistCompAgent(agent) / this.speed;
            //联盟邻居节点损失            
            return value;
        }


        /// <summary>
        /// 计算当前目标距离任务T的地理距离
        /// </summary>
        /// <param name="t">申请的任务</param>
        /// <returns></returns>
        public float DistComp(Task t)
        {
            //根据大圆航线距离进行计算
            double d;
            d = Math.Pow(Math.Abs(t.lat - curtLocattion.lat), 2) + Math.Pow(Math.Abs(t.lot - curtLocattion.lot), 2);
            return (float)Math.Pow(d, 0.5);
        }

        /// <summary>
        /// 计算当前智能体与其他智能体的地理距离
        /// </summary>
        /// <param name="a">智能体</param>
        /// <returns>返回智能体之间的距离</returns>
        public float DistCompAgent(Agent a)
        {
            double d;
            d = Math.Pow(Math.Abs(curtLocattion.lat - a.curtLocattion.lat), 2) + Math.Pow(Math.Abs(curtLocattion.lot - a.curtLocattion.lot), 2);
            return (float)Math.Pow(d, 0.5);
        }

        /// <summary>
        /// 计算两个智能体之间的距离
        /// </summary>
        /// <param name="a">智能体a</param>
        /// <param name="b">智能体b</param>
        /// <returns>返回两点间的距离</returns>
        private float Dist2Agent(Agent a, Agent b)
        {
            double d;
            d = Math.Pow(Math.Abs(a.curtLocattion.lat - b.curtLocattion.lat), 2) + Math.Pow(Math.Abs(a.curtLocattion.lot - b.curtLocattion.lot), 2);
            return (float)Math.Pow(d, 0.5);
        }
        /// <summary>
        ///当前智能体到达目标任务需要消耗的燃料
        /// </summary>
        /// <param name="t">申请的任务</param>
        /// <returns>需要用掉的燃料</returns>
        private float fuelComp(Task t)
        {
            float fuel = 0;
            this.loyalty = DistComp(t) / speed;
            fuel = this.loyalty * fuelCost;
            return fuel;
        }

        /// <summary>
        /// 计算当前agent的可通信节点，构建本地通信网络
        /// 认为通信网络是按照动态距离进行构建的
        /// 每个智能体的邻居节点数量上限是10
        /// </summary>
        /// <param name="agent_list">全体智能体集合</param>
        public void NeighborhoodCoalitionConstruct(List<Agent> agent_list, ref List<Neighbor> list_Neibor)
        {
            ///每个智能体添加的邻居节点十个为上限
            //List<Neighbor> neighborTemp_List = new List<Neighbor>(0);
            for (int i = 0; i < agent_list.Count; i++)
            {
                float dis = DistCompAgent(agent_list[i]);
                if (dis <= comRadio && dis != 0 && !IsAgentInNeighbors(agent_list[i],this))
                {
                    if (agent_list[i].list_Neighbors == null)
                    {
                        Neighbor nb = new Neighbor();
                        nb.agent= agent_list[i];
                        nb.agent.curtLocattion = agent_list[i].curtLocattion;
                        ///置信度的初始化
                        nb.belief = 0.5f;
                        nb.valueAsNeighbor = agent_list[i].agentNetValue;
                        list_Neibor.Add(nb);
                    }
                    else if (agent_list[i].list_Neighbors != null)
                    {
                        if (agent_list[i].list_Neighbors.Count < maxNeighborCount)
                        {
                            Neighbor nb = new Neighbor();
                            nb.agent = agent_list[i];
                            nb.agent.curtLocattion = agent_list[i].curtLocattion;
                            ///置信度的初始化
                            nb.belief = 0.5f;
                            nb.valueAsNeighbor = agent_list[i].agentNetValue;
                            list_Neibor.Add(nb);
                        }
                    }
                }
                else if (dis > comRadio && IsAgentInNeighbors(agent_list[i],this))
                {
                    Neighbor nb = new Neighbor();
                    nb.agent = agent_list[i];
                    nb.agent.curtLocattion = agent_list[i].curtLocattion;
                    list_Neibor.Remove(nb);
                }
            }
            this.agentNetValue = new agentUtilityInLocalNet();
            // this.list_Neighbors = list_Neibor;
            //自由度与邻居节点数量成反比，邻居数量越多，自由度越低
            //this.freedom = (float)(1 - list_Neibor.Count / maxNeighborCount);
            int choice = 3;
            switch (choice)
            {
                case 0:
                    if ((this.LocalCNA.eIa + this.LocalCNA.eIs) * this.cnaM.DC >= 1)
                    {
                        this.freedom = 0;
                    }
                    else
                        this.freedom = (float)(1 - (this.LocalCNA.eIa + this.LocalCNA.eIs) * this.cnaM.DC);
                    break;
                case 1:
                    if (this.cnaM.DC >= 1)
                        this.freedom = 0;
                    else
                        this.freedom = (float)(1-(this.cnaM.DC));
                    break;
                case 3:
                    if (this.cnaM.LocalR >= 1)
                        this.freedom = 0;
                    else
                        this.freedom = (float)(1 - (this.cnaM.LocalR));
                    break;
                default:
                    break; 
            }
        }

        /// <summary>
        /// 判断一个邻居节点是否在某智能体邻居节点集合内
        /// </summary>
        /// <param name="nb"></param>
        /// <param name="agent2"></param>
        /// <returns></returns>
        private bool IsNbInNeighbors(Neighbor nb, Agent agent2)
        {
            foreach (Neighbor n in agent2.list_Neighbors)
            {
                if (n.agent.ID == nb.agent.ID)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// 查询智能体a是否在本智能体的邻居节点内
        /// </summary>
        /// <param name="a">智能体a</param>
        /// <returns></returns>
        private bool IsAgentInNeighbors(Agent a, Agent goal)
        {
            for (int i = 0; i < goal.list_Neighbors.Count; i++)
            {
                if (a.ID == goal.list_Neighbors[i].agent.ID)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断当前合约是否已经在智能体a的选择合约中
        /// </summary>
        /// <param name="offer">需要转移的合约</param>
        /// <param name="aNumer">被转移的目标智能体</param>
        /// <returns></returns>
        private bool IsOfferInPotentialOffers(Offers offer, Agent a)
        {
            for (int i = 0; i < a.potential_Offers.Count; i++)
            {
                if (offer.leader.ID == a.potential_Offers[i].leader.ID)
                {
                    return true;
                }
            }
            return false;
        }
               
        /// <summary>
        /// 添加某节点作为网络邻居后自己的最佳位置
        /// </summary>
        /// <param name="neighborList">智能体全集</param>
        ///  <param name="aList">邻居节点集合</param>
        private Location OptimalLocationCompt( List<Agent> aList, List<Neighbor> neighborList)
        {
            List<Agent> neighborAgents = new List<Agent>(0);
            ///用于存储每次进行圆周距离计算的agent
            List<Agent> tempAgents = new List<Agent>(0);
            List<Location> locationList = new List<Location>(0);
            List<int> number = new List<int>(0);
            float distancceMax = 0;
            Random radom = new Random();

            ///用于标记最后最佳的结果
            Location optLocation = new Location();
            Location loc = new Location();

            ///用于标记新圆心坐标
            Location newCirclePt = new Location();

            //用于判断最远距离
            float tempdist = 0;
            int mark = 0;
            int j = 0;
            //将对应的邻居节点的位置形成点集
            foreach (Neighbor nb in neighborList)
            {
                neighborAgents.Add(nb.agent);
            }
            ///计算圆心
            ///随机抽取点集中三个点。随机抽取第一个点，并添加与其距离最远的一个点，并随机添加第三个点
            //int kk = typeDecide(neighborAgents);
            switch (neighborAgents.Count)
            {
                ///当邻居只有一个点时
                case 1:
                    newCirclePt = neighborAgents[0].curtLocattion;
                    float lot = (this.curtLocattion.lot + newCirclePt.lot) / 2;
                    float lat = (this.curtLocattion.lat + newCirclePt.lot) / 2;
                    optLocation.lot = lot;
                    optLocation.lat = lat;
                    return optLocation;
                case 2:
                    foreach (Agent a in neighborAgents)
                    {
                        locationList.Add(a.curtLocattion);
                    }
                    if( !locationList.Contains(this.curtLocattion))
                        locationList.Add(this.curtLocattion);
                    switch (locationList.Count)
                    {
                        case 2:
                            newCirclePt.lat = (locationList[0].lat + locationList[1].lat) / 2;
                            newCirclePt.lot = (locationList[0].lot + locationList[1].lot) / 2;
                            optLocation = newCirclePt;
                            break;
                        case 3:
                            newCirclePt = Circle_Center(locationList);
                            optLocation = newCirclePt;
                            break;
                        default:
                            newCirclePt = Circle_Center(locationList);
                            optLocation = newCirclePt;
                            break;
                    }                  
                    return optLocation;
                case 3:
                    foreach (Agent a in neighborAgents)
                    {
                        locationList.Add(a.curtLocattion);
                    }
                    newCirclePt = Circle_Center(locationList);
                    optLocation = newCirclePt;
                    return optLocation;
                //当邻居节点数量多于三个时
                default:
                    ///当neighborAgents点集中点的数量超过三个，则选出三个随机智能体
                    ///开始计算圆心
                    float R ;
                    do
                    {
                        ///选择三个随机智能体点
                        SelectPt(neighborAgents, ref tempAgents);
                        foreach (Agent a in tempAgents)
                        {
                            loc = a.curtLocattion;
                            locationList.Add(loc);
                        }
                        //根据选定的三个点进行圆心计算
                        newCirclePt = Circle_Center(locationList);
                        //计算点集中到圆心点最远距离，并记录该点的位置，记该点为D
                        R = Dist2Locations(locationList[0], newCirclePt);
                        distancceMax = Dist2Locations(neighborAgents[0].curtLocattion, newCirclePt);
                        for (int i = 1; i < neighborAgents.Count; i++)
                        {
                            tempdist = Dist2Locations(neighborAgents[i].curtLocattion, newCirclePt);
                            if (tempdist > distancceMax)
                            {
                                distancceMax = tempdist;
                                mark = i;
                            }
                        }
                        //如果最大距离也在通信范围以内，则，该点就是所求圆心
                        //如果不在，则另外选一个点，重新计算圆心
                        //locationList.Clear();
                        if (Math.Abs(distancceMax - R) <= 1)
                        {
                            optLocation = newCirclePt;
                            return optLocation;
                        }
                        else if(distancceMax < comRadio)
                        {
                            optLocation = newCirclePt;
                            return optLocation;
                        }
                        ///如果找到的圆点不满足需要，则取最远点d，重新进行计算
                        else
                        {
                            //将之前的三个点位置清除
                            locationList.Clear();
                            //将点D加入进行圆心计算
                            locationList.Add(neighborAgents[mark].curtLocattion);
                            //再在ABC三点中任取两个节点
                            for (int i = 0; i < tempAgents.Count; i++)
                            {
                               if (!IsAgentInLocationList(locationList, tempAgents[i].curtLocattion))
                                        locationList.Add(tempAgents[i].curtLocattion);
                                if (locationList.Count >= 3)
                                    break;
                            }
                            ///计算新的圆心
                            if (locationList.Count >= 3)
                            {
                                newCirclePt = Circle_Center(locationList);
                                //对点集到新圆心的距离进行计算，求出最大距离
                                R = Dist2Locations(locationList[0], newCirclePt);
                                distancceMax = Dist2Locations(neighborAgents[0].curtLocattion, newCirclePt);
                                for (int i = 1; i < neighborAgents.Count; i++)
                                {
                                    tempdist = Dist2Locations(neighborAgents[i].curtLocattion, newCirclePt);
                                    if (tempdist > distancceMax)
                                    {
                                        distancceMax = tempdist;
                                        mark = i;
                                    }
                                }
                                if (Math.Abs(distancceMax - R) <= 1)
                                {
                                    optLocation = newCirclePt;
                                    return optLocation;
                                }
                            }
                            else if(locationList.Count < 3)
                            {
                                //当点少于三个时，进行添加
                                for (int i = 0; i < neighborAgents.Count; i++)
                                {
                                    if (!IsAgentInLocationList(locationList,neighborAgents[i].curtLocattion))
                                    {
                                        locationList.Add(neighborAgents[i].curtLocattion);
                                    }
                                    if (locationList.Count >= 3)
                                        break;
                                }
                                //添加完了之后对计算集里面的点数进行分类计算                             
                                newCirclePt = Circle_Center(locationList);
                                //对点集到新圆心的距离进行计算，求出最大距离
                                R = Dist2Locations(locationList[0], newCirclePt);
                                distancceMax = Dist2Locations(neighborAgents[0].curtLocattion, newCirclePt);
                                for (int i = 1; i < neighborAgents.Count; i++)
                                {
                                    tempdist = Dist2Locations(neighborAgents[i].curtLocattion, newCirclePt);
                                    if (tempdist > distancceMax)
                                    {
                                        distancceMax = tempdist;
                                        mark = i;
                                    }
                                }
                                if (Math.Abs(distancceMax - R) <= 1)
                                {
                                    optLocation = newCirclePt;
                                    return optLocation;
                                }
                            }
                        }
                        tempAgents.Clear();
                    } while (Math.Abs(distancceMax - R) >1);             
                    return optLocation;
            } 
        }

        /// <summary>
        /// 布尔函数，判断位置a是否在列表中
        /// </summary>
        /// <param name="locationList">位置列表</param>
        /// <param name="a">位置a</param>
        /// <returns>如果在，返回true，如果不在，返回false</returns>
        private bool IsAgentInLocationList(List<Location> locationList, Location a)
        {
            bool flag = false;
            foreach(Location l in locationList)
            {
                if (l.lot == a.lot && l.lat == a.lot)
                    flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 返回智能体个数，当个数大于三个时，返回4
        /// </summary>
        /// <param name="agentList"></param>
        /// <returns></returns>
        private int typeDecide(List<Agent> agentList)
        {
            int k = 0;
            k = agentList.Count;
            if (k > 3)
            {
                k = 4;
            }
            return k;
        }

        /// <summary>
        /// 从多于三个点的点集中选择三个点
        /// </summary>
        /// <param name="agentList">被选择智能体集合</param>
        /// <param name="threeAgents">用于存放选择结果的智能体集合</param>
        private void SelectPt(List<Agent> agentList, ref List<Agent> threeAgents)
        {
            Random d = new Random();
            int k;
            //添加随机一个节点
            do
            {
                k = d.Next(agentList.Count);
                ///随机添加一个点，标记为A点，进行随机三个点的生成
                if (agentList[k].count != agentList[k].count - 1)
                {
                    threeAgents.Add(agentList[k]);
                    agentList[k].choseCount++;
                }
            } while (threeAgents.Count < 1);

            int mark = 0;
            float disT = Dist2Locations(threeAgents[0].curtLocattion, agentList[0].curtLocattion);
            for (int i = 1; i < agentList.Count; i++)
            {
                float tempD = Dist2Locations(threeAgents[0].curtLocattion, agentList[i].curtLocattion);
                if (tempD > disT)
                {
                    disT = tempD;
                    mark = i;
                }
            }
            //将与第一个智能体位置最远的点添加为B点
            threeAgents.Add(agentList[mark]);
            agentList[mark].choseCount++;
            //在排除ab两点之后，选择与b点最远的点进行添加  
            List<Agent> tempList = new List<Agent>();
            foreach (Agent a in agentList)
            {
                tempList.Add(a);
            }
            foreach (Agent a in threeAgents)
            {
                tempList.Remove(a);
            }       
            do
            {
                k = d.Next(tempList.Count);
                threeAgents.Add(tempList[k]);
            } while (threeAgents.Count<3);            
        }
        /// <summary>
        /// 返回离目标agent最远距离的agent
        /// </summary>
        /// <param name="agentList">智能体全集</param>
        /// <param name="a">目标智能体</param>
        /// <returns></returns>
        private Agent returnAgent(List<Agent> agentList,Agent a)
        {
            Agent ag = new Agent();
            int mark = 0;
            float disT = Dist2Locations(a.curtLocattion, agentList[0].curtLocattion);
            for (int i = 1; i < agentList.Count; i++)
            {
                float tempD = Dist2Locations(a.curtLocattion, agentList[i].curtLocattion);
                if (tempD > disT)
                {
                    disT = tempD;
                    mark = i;
                }
            }
            ag = agentList[mark];
            return ag;
        }
        /// <summary>
        /// 计算两个智能体之间的距离
        /// </summary>
        /// <param name="a">位置1</param>
        /// <param name="b">位置2</param>
        /// <returns></returns>
        private float Dist2Locations(Location a, Location b)
        {
            double d;
            d = Math.Pow(Math.Abs(b.lat - a.lat), 2) + Math.Pow(Math.Abs(b.lot - a.lot), 2);
            return (float)Math.Pow(d, 0.5);
        }

        /// <summary>
        /// 根据两个ID之间进行智能体之间的计算
        /// </summary>
        /// <param name="aList">智能体全集，用于查找ID所对应的的智能体</param>
        /// <param name="id1">ID1</param>
        /// <param name="id2">ID2</param>
        /// <returns>返回计算的距离</returns>
        private float Dist2ID(List<Agent> aList, int id1, int id2)
        {
            Agent a, b;
            a = new Agent();
            a.ID = id1;
            b = new Agent();
            b.ID = id2;
            foreach (Agent tat in aList)
            {
                if (tat.ID == a.ID)
                    a = tat;
                if (tat.ID == b.ID)
                    b = tat;
            }
            float dis = Dist2Agent(a, b);
            return dis;
        }
        /// <summary>
        /// 求三角形外接圆心坐标
        /// </summary>
        private Location Circle_Center(List<Location> agent_region)
        {
            Location newCenter = new Location();
            float x1, x2, x3, y1, y2, y3;
            double k;
            double a1, b1, a2, b2, a3, b3;
            float x = 0;
            float y = 0;
            float radiu = 0;
            switch (agent_region.Count)
            {
                case 1:
                    x1 = agent_region[0].lot;
                    y1 = agent_region[0].lat;
                    x = x1;
                    y = y1;
                    break;
                case 2:
                    x1 = agent_region[0].lot;
                    x2 = agent_region[1].lot;
                    y1 = agent_region[0].lat;
                    y2 = agent_region[1].lat;
                    x = (x1 + x2) / 2;
                    y = (y1 + y2) / 2;
                    break;
                default:
                    //当agent_region中点数量>=3时
                  
                  
                    x1 = agent_region[0].lot;
                    x2 = agent_region[1].lot;
                    x3 = agent_region[2].lot;
                    y1 = agent_region[0].lat;
                    y2 = agent_region[1].lat;
                    y3 = agent_region[2].lat;

                    a1 = (x1 - x3);
                    a2 = x2 - x1;
                    a3 = x3 - x2;
                    b1 = y1 - y3;
                    b2 = y2 - y1;
                    b3 = y3 - y2;
                    k = b3 * a2 - b2 * a3;
                    x = (float)((-1) * (b1 * (x2 * x2 + y2 * y2) + b2 * (x3 * x3 + y3 * y3) + b3 * (x1 * x1 + y1 * y1)) / (2 * k));
                    //((y2 - y1) * (y3 * y3 - y1 * y1 + x3 * x3 - x1 * x1) - (y3 - y1) * (y2 * y2 - y1 * y1 + x2 * x2 - x1 * x1)) / (2 * (x3 - x1) * (y2 - y1) - 2 * ((x2 - x1) * (y3 - y1)));
                    y = (float)((a1 * (x2 * x2 + y2 * y2) + a2 * (x3 * x3 + y3 * y3) + a3 * (x1 * x1 + y1 * y1)) / (2 * k));
                    ///((x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1) - (x3 - x1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)) / (2 * (y3 - y1) * (x2 - x1) - 2 * ((y2 - y1) * (x3 - x1)));

                  
                    break;
            }
            newCenter.lot = x;
            newCenter.lat = y;
            radiu = (agent_region[0].lot - x) * (agent_region[0].lot - x) + (agent_region[0].lat - y) * (agent_region[0].lat - y);
            return newCenter;
        }


        /// <summary>
        /// 对当前节点的网络状态进行评价
        /// </summary>
        /// <param name="aList">全体智能体</param>
        /// <param name="a">待判断智能体</param>
        private void NetStateJudge(Agent a)
        {
            int numNeighbor = 0;
            numNeighbor = a.list_Neighbors.Count;
            int numSubNeighbor = 0;
            foreach (Neighbor b in list_subNeighbors)
            {
                if (!IsNbInNeighbors(b, a))
                {
                    numSubNeighbor++;
                }
            }
            if (numNeighbor < 4 && numSubNeighbor < 5)
                a.netStatus = NetStatus.Sparse_Network;
            else
                a.netStatus = NetStatus.Crowded_Network;
        }

        /// <summary>
        /// 生成关于本智能体向智能体a发送关于任务t的合约
        /// </summary>
        /// <param name="a">协商智能体变量</param>
        /// <param name="task">合约面向的任务</param>
        /// <param name="OT">合约的有效时间</param>
        /// <returns>返回需要合作的offer</returns>
        protected Offers GenerateOffers(Agent a, Task task, int OT)
        {
            ///当智能体空闲时，先将智能体全部加入联盟
            Offers offer = new Offers();
            offer.leader = task.potential_leader;
            offer.resp = false;
            offer.PassTime = 0;
            offer.task = task;
            offer.offerT = OT;
            ablity requireM = new ablity();
            if ((a.statusInCoalition == agentStatusInCoalition.IdleAgent && !IsAgentInCoalitionYet(a, task)) || offer.leader == a)
            //提供一个offer给agent
            {
                if (task.pre_members.Count <= 0)
                    offer.pay = (float)(task.reward * (0.5 * (float)(a.abilityA.ablityX / task.requirement.ablityX) + 0.5 * (float)(a.abilityA.ablityY / task.requirement.ablityY)));
                if (task.pre_members != null && task.pre_members.Count > 0)
                ///先到先得，获得收益多
                {
                    requireM = task.coaRequireCheck(task.pre_members);
                    if (a.abilityA.ablityX <= requireM.ablityX && a.abilityA.ablityY <= requireM.ablityY)
                    {
                        offer.pay = (float)(task.reward * (0.5 * (a.abilityA.ablityX / task.requirement.ablityX) + 0.5 * (a.abilityA.ablityY / task.requirement.ablityY)));
                    }
                    //当任务只剩下一点的时候，只需要最后一个智能体出部分的能力
                    else if (a.abilityA.ablityX > requireM.ablityX || a.abilityA.ablityY > requireM.ablityY)
                    {
                        offer.pay = (float)(task.reward * (0.5 * requireM.ablityX + 0.5 * requireM.ablityY));
                        //offer.pay = (float)(task.reward * (0.5 * (a.abilityA.ablityX / task.requirement.ablityX) + 0.5 * (a.abilityA.ablityY / task.requirement.ablityY))); ;
                    }
                }
            }
            return offer;
        }

        /// <summary>
        /// 对于智能体收到的offer进行比较，从而做出选择
        /// </summary>
        /// <returns>返回确定的offer编号</returns>
        public int ResponsOffer(ref List<Task> task)
        {
            double payoff = -1;
            int mark = 0;
            if (potential_Offers.Count > 0)
            {
                for (int i = 0; i < potential_Offers.Count; i++)
                {
                    for (int j = 0; j < task.Count; j++)
                    {
                        //如果并不存在在预备联盟中
                        if (!IsAgentInCoalitionYet(this, task[j]))
                        {
                            potential_Offers[i].cost = (float)((DistComp(task[j]) / speed) * fuelCost);
                            if (payoff > potential_Offers[i].pay - potential_Offers[i].cost)
                            {
                                payoff = potential_Offers[i].pay - potential_Offers[i].cost;
                                mark = i;
                            }
                        }
                    }
                }
                if (payoff != -1 && potential_Offers.Count != 1)
                {
                    return mark;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        /// <summary>
        /// 判断agent是否已经在task的预备联盟中，如果是，返回true
        /// </summary>
        /// <param name="a">智能体</param>
        /// <param name="f">offer</param>
        /// <returns></returns>
        protected bool IsAgentInCoalitionYet(Agent a, Task task)
        {
            if (task == null)
                return false;
            for (int i = 0; i < task.pre_members.Count; i++)
            {
                if (a.ID == task.pre_members[i].agent.ID)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///根据智能体当前状态，更新执行任务智能体位置状态
        /// </summary>
        public void UpDateAgentLocation(int step)
        {
            float dis = 0;
            float x = 0;
            float y = 0;
            switch (statusInCoalition)
            {
                case agentStatusInCoalition.CoalitionAgent: //当智能体已经参与某联盟，并已经被成功分配任务，正在移动到任务点
                    if (taskTaken != null && (curtLocattion.lat != taskTaken.lat || curtLocattion.lot != taskTaken.lot)) //当运动到目标点则停止更新坐标
                    {
                        dis = DistComp(taskTaken);
                        x = ((speed / dis) * (taskTaken.lat - curtLocattion.lat)) + curtLocattion.lat;
                        y = ((speed / dis) * (taskTaken.lot - curtLocattion.lot)) + curtLocattion.lot;
                        if (Math.Abs(x - taskTaken.lat) < speed && Math.Abs(y - taskTaken.lot) < speed)
                        {
                            curtLocattion.lat = taskTaken.lat;
                            curtLocattion.lot = taskTaken.lot;
                            //如果是因为执行任务而进行的移动的智能体
                            if (this.statusInCoalition == agentStatusInCoalition.CoalitionAgent)
                            {
                                this.statusInCoalition = agentStatusInCoalition.ExecutingAgent;
                                this.taskTaken.arrive_time = step;
                                this.taskTaken.excuting_counter = 0;
                            }
                        }
                        else if (Math.Abs(y - taskTaken.lot) < speed && Math.Abs(x - taskTaken.lat) >= speed)
                        {
                            curtLocattion.lat = x;
                            curtLocattion.lot = taskTaken.lot;
                        }
                        else if (Math.Abs(y - taskTaken.lot) >= speed && Math.Abs(x - taskTaken.lat) < speed)
                        {
                            curtLocattion.lat = taskTaken.lat;
                            curtLocattion.lot = y;
                        }
                        else
                        {
                            curtLocattion.lat = x;
                            curtLocattion.lot = y;
                        }
                    }
                    break;
                case agentStatusInCoalition.WaitingAgent:  //当智能体处于联盟协同构建中
                    this.statusInCoalition = agentStatusInCoalition.IdleAgent;
                    break;
                case agentStatusInCoalition.IdleAgent:  //当智能体处于空闲状态，则进行原地等待

                    break;
                case agentStatusInCoalition.ExecutingAgent:
                    this.taskTaken.excuting_counter++;
                    if (this.taskTaken.excuting_counter >= this.taskTaken.period_time)
                    {
                        this.statusInCoalition = agentStatusInCoalition.IdleAgent;
                        this.taskTaken.status = Task.taskStatus.completed;
                    }
                    break;
                case agentStatusInCoalition.AdaptionInNetAgent: //当智能体处于网络自适应调节中
                    if ((curtLocattion.lat != destLoc.lat && curtLocattion.lot != destLoc.lot)) //当运动到目标点则停止更新坐标
                    {
                        dis = Dist2Locations(curtLocattion, destLoc);
                        x = ((speed / dis) * (destLoc.lat - curtLocattion.lat)) + curtLocattion.lat;
                        y = ((speed / dis) * (destLoc.lot - curtLocattion.lot)) + curtLocattion.lot;
                        //当两点之间距离在一个speed之内，认为已经到达
                        if (Math.Abs(x - destLoc.lat) < speed && Math.Abs(y - destLoc.lot) < speed)
                        {
                            curtLocattion.lat = destLoc.lat;
                            curtLocattion.lot = destLoc.lot;
                            //到达之后进行状态修改                 
                            this.statusInCoalition = agentStatusInCoalition.IdleAgent;
                        }
                        else if (Math.Abs(y - destLoc.lot) < speed && Math.Abs(x - destLoc.lat) >= speed)
                        {
                            curtLocattion.lat = x;
                            curtLocattion.lot = destLoc.lot;
                        }
                        else if (Math.Abs(y - destLoc.lot) >= speed && Math.Abs(x - destLoc.lat) < speed)
                        {
                            curtLocattion.lat = destLoc.lat;
                            curtLocattion.lot = y;
                        }
                        else
                        {
                            curtLocattion.lat = x;
                            curtLocattion.lot = y;
                        }
                    }
                    //如果是x或者y轴不同时到达
                    #region 可能多余代码
                    else if ((Math.Abs(curtLocattion.lat - destLoc.lat) < speed) || (Math.Abs(curtLocattion.lot - destLoc.lot) < speed)) //当运动到目标点则停止更新坐标
                    {
                        //如果是x轴先到达
                        if (Math.Abs(curtLocattion.lat - destLoc.lat) < speed)
                        {
                            x = curtLocattion.lat;
                            y = curtLocattion.lot + speed;
                            //当两点之间距离在一个speed之内，认为已经到达
                            if (Math.Abs(x - destLoc.lat) < speed && Math.Abs(y - destLoc.lot) < speed)
                            {
                                curtLocattion.lat = destLoc.lat;
                                curtLocattion.lot = destLoc.lot;
                                //到达之后进行状态修改                 
                                this.statusInCoalition = agentStatusInCoalition.IdleAgent;
                            }
                            else if (Math.Abs(x - destLoc.lat) < speed && Math.Abs(y - destLoc.lot) >= speed)
                            {
                                curtLocattion.lat = destLoc.lat;
                                curtLocattion.lot = y;
                            }
                            else if (Math.Abs(x - destLoc.lat) < speed && Math.Abs(y - destLoc.lot) < speed)
                            {
                                curtLocattion.lat = x;
                                curtLocattion.lot = destLoc.lot;
                            }
                            else if (Math.Abs(x - destLoc.lat) >= speed && Math.Abs(y - destLoc.lot) >= speed)
                            {
                                curtLocattion.lat = x;
                                curtLocattion.lot = y;
                            }
                        }
                        //如果y坐标到达，x轴坐标还没到达
                        else if (Math.Abs(curtLocattion.lot - destLoc.lot) < speed)
                        {
                            y = destLoc.lot;
                            x = curtLocattion.lat + speed;
                            if (Math.Abs(x - destLoc.lat) < speed && Math.Abs(y - destLoc.lot) < speed)
                            {
                                curtLocattion.lat = destLoc.lat;
                                curtLocattion.lot = destLoc.lot;
                                //到达之后进行状态修改                 
                                this.statusInCoalition = agentStatusInCoalition.IdleAgent;
                            }
                            else if (Math.Abs(x - destLoc.lat) >= speed && Math.Abs(y - destLoc.lot) >= speed)
                            {
                                curtLocattion.lat = x;
                                curtLocattion.lot = y;
                            }
                            else if (Math.Abs(x - destLoc.lat) >= speed && Math.Abs(y - destLoc.lot) < speed)
                            {
                                curtLocattion.lat = x;
                                curtLocattion.lot = destLoc.lot;
                            }
                            else
                                  if (Math.Abs(x - destLoc.lat) < speed && Math.Abs(y - destLoc.lot) >= speed)
                            {
                                curtLocattion.lat = destLoc.lat;
                                curtLocattion.lot = y;
                            }
                        }
                    }
                    #endregion 可能多余代码
                    //移动完毕进行智能体状态更新，释放到达目标点的智能体
                    //if(Math.Abs(curtLocattion.lat - destLoc.lat)<speed && Math.Abs(curtLocattion.lot- destLoc.lot)<speed)
                    //{
                    //每各时间片内调整完就进行智能体释放
                    this.statusInCoalition = agentStatusInCoalition.IdleAgent;
                    //}
                        break;

                    default:
                    break;
            }
        }

        /// <summary>
        /// 智能体移动函数
        /// </summary>
        /// <param name="curLct">当前位置</param>
        /// <param name="destLct">目标位置</param>
        private void agentMove(ref Location curLct, Location destLct,int step)
        {
            float dis = 0;
            float x, y;
            if ((curLct.lat != destLct.lat && curLct.lot != destLct.lot)) //当运动到目标点则停止更新坐标
            {
                dis = Dist2Locations(curLct, destLct);
                x = ((speed / dis) * (destLct.lat - curLct.lat)) + curLct.lat;
                y = ((speed / dis) * (destLct.lot - curLct.lot)) + curLct.lot;
                //当两点之间距离在一个speed之内，认为已经到达
                if (Math.Abs(x - destLct.lat) < speed && Math.Abs(y - destLct.lot) < speed)
                {
                    curLct.lat = destLct.lat;
                    curLct.lot = destLct.lot;
                }
                else if (Math.Abs(y - destLct.lot) < speed && Math.Abs(x - destLct.lat) >= speed)
                {
                    curLct.lat = x;
                    curLct.lot = destLct.lot;
                }
                else if (Math.Abs(y - destLct.lot) >= speed && Math.Abs(x - destLct.lat) < speed)
                {
                    curLct.lat = destLct.lat;
                    curLct.lot = y;
                }
                else
                {
                    curLct.lat = x;
                    curLct.lot = y;
                }
            }
            else if ((Math.Abs(curLct.lat - destLct.lat) < speed) || (Math.Abs(curLct.lot - destLct.lot) < speed)) //当运动到目标点则停止更新坐标
            {
                //如果是x轴先到达
                if (Math.Abs(curLct.lat - destLct.lat) < speed)
                {
                    x = curLct.lat;
                    y = curLct.lot + speed;
                    //当两点之间距离在一个speed之内，认为已经到达
                    if (Math.Abs(x - destLct.lat) < speed && Math.Abs(y - destLct.lot) < speed)
                    {
                        curLct.lat = destLct.lat;
                        curLct.lot = destLct.lot;
                    }
                    else if (Math.Abs(x - destLct.lat) < speed && Math.Abs(y - destLct.lot) >= speed)
                    {
                        curLct.lat = destLct.lat;
                        curLct.lot = y;
                    }
                    else if (Math.Abs(x - destLct.lat) < speed && Math.Abs(y - destLct.lot) < speed)
                    {
                        curLct.lat = x;
                        curLct.lot = destLct.lot;
                    }
                    else if (Math.Abs(x - destLct.lat) >= speed && Math.Abs(y - destLct.lot) >= speed)
                    {
                        curLct.lat = x;
                        curLct.lot = y;
                    }
                }
                //如果y坐标到达，x轴坐标还没到达
                else if (Math.Abs(curLct.lot - destLct.lot) < speed)
                {
                    y = destLct.lot;
                    x = curLct.lat + speed;
                    if (Math.Abs(x - destLct.lat) < speed && Math.Abs(y - destLct.lot) < speed)
                    {
                        curLct.lat = destLct.lat;
                        curLct.lot = destLct.lot;
                    }
                    else if (Math.Abs(x - destLct.lat) >= speed && Math.Abs(y - destLct.lot) >= speed)
                    {
                        curLct.lat = x;
                        curLct.lot = y;
                    }
                    else if (Math.Abs(x - destLct.lat) >= speed && Math.Abs(y - destLct.lot) < speed)
                    {
                        curLct.lat = x;
                        curLct.lot = destLct.lot;
                    }
                    else
                          if (Math.Abs(x - destLct.lat) < speed && Math.Abs(y - destLct.lot) >= speed)
                    {
                        curLct.lat = destLct.lat;
                        curLct.lot = y;
                    }
                }
                //到达之后进行状态修改      
                //如果是智能体联盟移动 
                if (this.statusInCoalition == agentStatusInCoalition.CoalitionAgent)
                {
                    this.statusInCoalition = agentStatusInCoalition.ExecutingAgent;
                    this.taskTaken.arrive_time = step;
                    this.taskTaken.excuting_counter = 0;
                }
                //如果是因为网络调整进行移动的智能体
                if (this.statusInCoalition == agentStatusInCoalition.AdaptionInNetAgent)
                {
                    this.statusInCoalition = agentStatusInCoalition.IdleAgent;
                }
            }
        }


        /// <summary>
        /// 寻找巡航的下一目标点
        /// </summary>
        /// <returns></returns>
        private float[,] cruiseTarget()
        {
            float[,] center = new float[1, 1];
            float x, y;
            x = y = 0;
            foreach (Neighbor nb in list_Neighbors)
            {
                ;
            }
            center.SetValue(x, 0);
            center.SetValue(y, 1);
            return center;
        }
        /// <summary>
        /// 任务应用，待完成
        /// </summary>
        /// <param name="a">智能体</param>
        /// <param name="t">任务</param>
        /// <returns></returns>
        public bool applyTask(Agent a, Task t)
        {
            bool a1 = (a.loyalty >= t.call_time);
            bool a2 = (a.statusInCoalition == agentStatusInCoalition.IdleAgent);
            if (a1 & a2)
                return true;
            else return false;
        }

        /// <summary>
        /// 后续再进行操作
        /// </summary>
        private void CommunicationKeyPoint()
        {
            int count = 0;
            float abilitX = 0;
            float abilitY = 0;
            for (int j = 0; j < list_Neighbors.Count; j++)
            {
                //if (list_NeighborsNumber[j])
                {
                    count++;//记录当前智能体的活跃可用邻居，作为评价其关键点的依据之一
                    //abilitX += list_NeighborsNumber[j].ablityA.ablityX;
                    //abilitY += list_NeighborsNumber[j].ablityA.ablityY;
                }
            }
            //关键通信点的值计算
            keyPointValue = (float)count + abilitX + abilitY;
        }

        /// <summary>
        /// 对于承担某任务联盟召集工作的本智能体负责联盟构建,给每个空闲邻居节点发送合作协议，每个空闲邻居接受当前合作协议
        /// </summary>
        public void SendOffers(ref List<Agent> aList, int number, Task task, int step)
        {
            if (task != null)
            {
                //如果当前智能体已经在联盟内，状态已经改变，但合约还没生成则给当前智能体自己生成合约                
                if (aList[number].potential_Offers.Count == 0 && task.potential_leader == this)
                    aList[number].potential_Offers.Add(GenerateOffers(this, task, step));
                //对所有周围邻居节点都进行合约发送
                for (int i = 0; i < list_Neighbors.Count; i++)
                {
                    //对其每个邻居点与它空闲的邻居节点进行通信以进行联盟构建，任务还没有完成联盟构建  
                    for (int k = 0; k < aList.Count; k++)
                    {
                        if ((aList[k].statusInCoalition == agentStatusInCoalition.IdleAgent || aList[i].statusInCoalition == agentStatusInCoalition.WaitingAgent)
                            && IsAgentInNeighbors(aList[k], list_Neighbors[i].agent) && !IsAgentInCoalitionYet(aList[k], task)) //对于邻居节点进行操作
                        {
                            aList[k].potential_Offers.Add(GenerateOffers(aList[k], task, step));
                        }
                    }
                }
                ///消息也转发到所有的次邻居节点，并通知他们进行联盟构成
                for (int i = 0; i < list_subNeighbors.Count; i++)
                {
                    for (int k = 0; k < aList.Count; k++)
                    {
                        if (aList[k].statusInCoalition != agentStatusInCoalition.CoalitionAgent && aList[k].ID == list_subNeighbors[i].agent.ID && !IsAgentInCoalitionYet(aList[k], task))
                        {
                            aList[k].potential_Offers.Add(GenerateOffers(aList[k], task, step));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 对到期没有给出回应的offer进行清除
        /// </summary>
        /// <param name="step"></param>
        public void OfferOrginize( int step)
        {
            Offers newF = new Offers();
            List<Offers> temp_offers = new List<Offers>();
            //记录要去掉的合约
            if (this != null && this.potential_Offers.Count != 0)
            {
                foreach (Offers f in this.potential_Offers)
                {
                    if (f.offerT <= step && f.resp == false)
                    {
                        newF = f;
                        temp_offers.Add(newF);
                    }
                }
            }
            //对合约进行处理
            foreach(Offers f in temp_offers)
            {
                if (potential_Offers.Contains(f))
                    potential_Offers.Remove(f);
             }
        }

        /// <summary>
        /// 当合约被拒绝之后，进行扩散传播
        /// </summary>
        /// <param name="offer">被拒绝的合同</param>
        private Offers TransferOffer(Offers offer, Agent a)
        {
            if (list_Neighbors.Count > 0)
            {
                for (int i = 0; i < list_Neighbors.Count; i++)
                {

                    if (!IsOfferInPotentialOffers(offer, a))
                    {
                        offer.PassTime++;
                        offer.pay = offer.pay * (offer.PassTime) / (1 + offer.PassTime);
                        return offer;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 每个智能体根据自己收到的合作协议，进行合作意向选择,同时将确定意向的智能体添加到备选联盟成员中
        /// 将拒绝的offer转发到
        /// </summary>
        /// <param name="taskList">任务集合的地址</param>
        /// <param name="aList">智能体集合的地址传递</param>
        /// <param name="number">智能体编号</param>
        public void OfferChooseAndTransfer(ref List<Task> taskList, ref List<Agent> aList, int number)
        {
            int mark = 0;
            //确定选择的offer             
            if (potential_Offers.Count >= 1)
            {
                if (potential_Offers.Count == 1)
                {
                    potential_Offers[mark].resp = true;
                    aList[number].potential_Offers[mark].resp = true;
                }
                else if (potential_Offers.Count > 1)
                {
                    //进行合约选择
                    mark = ResponsOffer(ref taskList);
                    if (mark > 0)
                    {
                        potential_Offers[mark].resp = true;
                        aList[number].potential_Offers[mark].resp = true;
                    }
                    //如果                      
                }
            }
            for (int i = 0; i < potential_Offers.Count; i++)
            {
                if (potential_Offers[i].resp == true)
                {
                    for (int j = 0; j < taskList.Count; j++)
                    {
                        if (taskList[j].ID == potential_Offers[mark].task.ID && !IsAgentInCoalitionYet(this, taskList[j]))
                        {
                            potential_Offers[i].aTofAgent = DistComp(taskList[j]) / speed;
                            //将智能体添加到对应任务的备选联盟成员名单
                            CoaMembers m = new CoaMembers();
                            m.agent = this;
                            m.rank = 0;
                            m.valueOfAgent = 0;
                            taskList[j].pre_members.Add(m);
                            statusInCoalition = agentStatusInCoalition.WaitingAgent;//更改智能体状态
                            break;
                        }
                    }
                }
                //如果智能体没有接受该项合作协议
                if (potential_Offers[i].resp != true)
                {
                    for (int j = 0; j < list_Neighbors.Count; j++)
                    {
                        for (int k = 0; k < aList.Count; k++)
                        {
                            Offers offer = TransferOffer(potential_Offers[i], aList[k]);
                            //当智能体没有确定承担任务时，
                            if (list_Neighbors[j].agent.ID == aList[k].ID && aList[k].statusInCoalition != agentStatusInCoalition.CoalitionAgent && offer != null)
                                aList[k].potential_Offers.Add(offer);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 对于能力超过的联盟，进行分解
        /// </summary>
        private void CoalitionSplit(float x, float y)
        {
            float x1, y1;
            int i = 0;
            do
            {
                x1 = x - taskTaken.pre_members[i].agent.abilityA.ablityX;
                y1 = y - taskTaken.pre_members[i].agent.abilityA.ablityY;
                i++;
                if (x1 == taskTaken.requirement.ablityX && y1 == taskTaken.requirement.ablityY)
                {
                    taskTaken.pre_members.Remove(taskTaken.pre_members[i]);
                }
            } while (x1 <= taskTaken.requirement.ablityX && y1 <= taskTaken.requirement.ablityY && i < taskTaken.potential_leader.list_Neighbors.Count);
        }
        #endregion

        /// <summary>
        /// 自适应网络结构调整
        /// 网络结构调整应当分为两部分，1是智能体稀疏分布，无法构建成能完成任务的联盟
        /// 2是当网络过于密集之后，需要对位置进行调整，例如当任务完成后都分布在任务点。此时对任务信息容易导致信息阻塞
        /// </summary>
        /// <param name="agent_list">全体智能体列表</param>
        /// <param name="Center">任务区域的中心</param>
        public void AdaptionInNetStructure(List<Agent> agent_list, Location Center)
        {
            List<float> contribution = new List<float>(0);
            //次邻居节点集合，即策略集合
            List<Neighbor> subNeighbor = new List<Neighbor>(0);
            ///构建次邻居节点集，即策略空间集合
            ///更新次邻居节点集合
            CoalitionOfSubNeighborsConstruct(ref subNeighbor);
            CoalitionOfSubNeighborsConstruct(ref list_subNeighbors);
            //计算当前智能体和邻居节点的邻居集中的重叠度
            OverLappingCptInNH(this, agent_list);
            //对当前网络状态进行判断
            NetStateJudge(this);
            ///获得本智能体的策略空间
            ///根据智能体所处网络状态进行调整
            switch(netStatus)
            {
                //拥挤网络调整，调整邻居节点，同去掉一个重叠度最多的邻居节点，添加一个重叠度最小的次邻居节点
                case NetStatus.Crowded_Network:
                    CrowdedNetAdaptionInTopo(this, agent_list);
                    break;
                    ///稀疏网络的调整方法
                case NetStatus.Sparse_Network:
                    switch (subNeighbor.Count)
                    {
                        case 0: ///当没有次邻居节点时，则有两项选择，当邻居节点数量少于4时，则往任务区域中心位置移动，二是进行自我位置调整
                            if (this.list_Neighbors.Count < 4)
                            {
                                this.chooseFlag = true;
                                this.leaveCount++;
                                //沿着往任务间区域中心位置的方向前进，前进一个通信半径
                                this.destLoc = RadioPtCpt(this.curtLocattion, Center);
                                //接受位置移动安排，进行移动，状态改为AdaptionInNetAgent
                                //移动结束则对状态修改至IDLE
                                this.statusInCoalition = agentStatusInCoalition.AdaptionInNetAgent;
                                this.count++;
                            }
                            else
                            {
                                this.chooseFlag = true;
                                Location la = OptimalLocationCompt(agent_list, list_Neighbors);
                                this.destLoc = la;
                                this.statusInCoalition = agentStatusInCoalition.AdaptionInNetAgent;
                                this.count++;
                            }
                            break;
                        default: ///当有一个以上的策略时，则进行策略选择
                            //将次邻居节点转换成对应的策略
                            if (subNeighbor.Count != 0)
                            {
                                strategy_List = new List<Strategy>(0);
                                foreach (Neighbor b in list_subNeighbors)
                                {
                                    Strategy st = new Strategy();
                                    st.belief = 0.5f;
                                    st.cost = 0;
                                    st.flag = false;
                                    st.item_Neighbor = b;
                                    strategy_List.Add(st);
                                }
                                //StrategyUpDate(agent_list);
                            }
                            ///策略选择以及对应的概率计算和更新 
                            BeliefUpDate(strategy_List, agent_list);
                            if (strategy_List.Count != 0)
                            {
                                StrategyWelfareCompute(agent_list, this.list_Neighbors);
                                //选择了当前
                                foreach (Strategy s in strategy_List)
                                {
                                    if (s.flag == true)
                                    ///进行最佳策略和当前位置的收益比较，看是否要进行位置移动
                                    {
                                        Agent a1 = new Agent();
                                        //指针
                                        a1.curtLocattion = this.curtLocattion;
                                        a1.abilityA = this.abilityA;
                                        a1.agentNetValue = this.agentNetValue;
                                        a1.chooseFlag = this.chooseFlag;
                                        a1.comRadio = this.comRadio;
                                        a1.count = this.count;
                                        a1.cruiseSpeed = this.cruiseSpeed;
                                        a1.freedom = this.freedom;
                                        a1.destLoc = this.destLoc;
                                        a1.ID = this.ID;
                                        a1.keyPointValue = this.keyPointValue;
                                        a1.leaveCount = this.leaveCount;
                                        a1.list_Neighbors = this.list_Neighbors;
                                        a1.list_subNeighbors = this.list_subNeighbors;
                                        a1.speed = this.speed;
                                        a1.statusInCoalition = this.statusInCoalition;
                                        a1.netStatus = this.netStatus;
                                        a1.strategy_List = this.strategy_List;
                                        a1.taskTaken = this.taskTaken;

                                        a1.curtLocattion = s.locationOpt;
                                        a1.list_Neighbors = new List<Neighbor>(0);
                                        //移动到最佳位置之后，进行邻居节点集的计算
                                        a1.NeighborhoodCoalitionConstruct(agent_list, ref a1.list_Neighbors);
                                        agentUtilityInLocalNet fWelfare = valueOfNeighborhoodCoalition(a1);
                                        ///当减去移动成本后，所获得的收益仍然高于当前所在邻域网的收益，则选择该策略，并进行移动
                                        ///暂存的welfare中已经进行成本计算
                                        if (fWelfare.ContributionInNet > this.agentNetValue.ContributionInNet)
                                        {
                                            this.chooseFlag = true;
                                            this.leaveCount++;
                                            this.destLoc = s.locationOpt;
                                            //接受位置移动安排，进行移动，状态改为AdaptionInNetAgent
                                            //移动结束则对状态修改至IDLE
                                            this.statusInCoalition = agentStatusInCoalition.AdaptionInNetAgent;
                                        }
                                        this.count++;
                                    }
                                }
                            }
                            strategy_List.Clear();
                            break;
                    }
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// 对智能体a1进行拥挤拓扑结构调整
        /// </summary>
        /// <param name="a1">被调整智能体</param>
        private void CrowdedNetAdaptionInTopo(Agent a1, List<Agent> agent_list)
        {
            int markMaxID = 0;
            float minUC = 10;
            float maxUC = 0;
            int markMinID = 0;
            List<Neighbor> overLapAgent = new List<Neighbor>(0);
            //将与A1邻居节点有重合的智能体归算到一个集合
            if (a1.list_Neighbors.Count > 0)
            {
                foreach (Neighbor b in a1.list_Neighbors)
                {
                    if (b.overlappingNumInN > 0)
                        overLapAgent.Add(b);
                }
            }
            //根据重叠
            switch (overLapAgent.Count)
            {
                case 0:
                    ///计算每一个邻居节点去掉智能体a1之后其utility变化比
                    foreach (Neighbor b in a1.list_Neighbors)
                    {
                        ComptUtilityCP(b, a1);
                        if (b.utilityCP < minUC)
                        {
                            minUC = b.utilityCP;
                            markMinID = b.agent.ID;
                        }
                    } 
                    ///添加一个对其帮助最大的智能体？
                    break;
                case 1:
                    break;
                default:
                    foreach (Neighbor b in a1.list_Neighbors)
                    {
                        ComptUtilityCP(b, a1);
                        if (b.utilityCP < minUC)
                        {
                            minUC = b.utilityCP;
                            markMinID = b.agent.ID;
                        }
                    }
                    break;
            }
            List<Neighbor> temp_Neighbors = new List<Neighbor>(0);
            //去掉指针影响
            foreach (Neighbor nb in a1.list_Neighbors)
            {
                Neighbor newN = new Neighbor();
                newN = nb;
                temp_Neighbors.Add(newN);
            }
            //去掉重叠率最多的智能体
            foreach (Neighbor nb in temp_Neighbors)
            {
                if (nb.agent.ID == markMaxID)
                {
                    temp_Neighbors.Remove(nb);
                    break;
                }
            }
            //添加重叠最低的智能体
            //foreach (Neighbor nb in a1.list_subNeighbors)
            //{
            //    if (nb.agent.ID == markMinID)
            //    {
            //        temp_Neighbors.Add(nb);
            //        break;
            //    }
            //}
            Location locNew = new Location();
            locNew = OptimalLocationCompt(agent_list, temp_Neighbors);
            a1.destLoc = locNew;
            a1.statusInCoalition = agentStatusInCoalition.AdaptionInNetAgent;
            a1.count++;
            a1.chooseFlag = true;
            //coalition的调整，从重合度高的，选择一个重合度不高的subneighbor进行邻域节点的调整
        }

       /// <summary>
       /// 智能体a1去掉邻居节点R之后utility的变化率
       /// </summary>
       /// <param name="a1">目标智能体</param>
       /// <param name="RB">需要去掉的邻居节点RB</param>
        private void ComptUtilityCP(Neighbor aN,Agent RB)
        {
            Agent a1 = new Agent();
            a1 = aN.agent;
            //不影响其他节点计算，定义一个临时变量
            Agent tempRB = new Agent();
            tempRB = RB;
            agentUtilityInLocalNet orginalU = new agentUtilityInLocalNet();
            orginalU = a1.agentNetValue;
            agentUtilityInLocalNet newU = new agentUtilityInLocalNet();
            if (IsAgentInNeighbors(tempRB, a1))
            {
                Neighbor rvN = new Neighbor();
                foreach (Neighbor b in tempRB.list_Neighbors)
                {
                    //找到需要被删除的智能体
                    if (b.agent.ID == tempRB.ID)
                        rvN = b;
                }
                tempRB.list_Neighbors.Remove(rvN);
                tempRB.list_subNeighbors.Clear();
                //重新构建智能体的次邻居节点集，以计算去掉指定邻居节点之后的邻居集
                CoalitionOfSubNeighborsConstruct(ref a1.list_subNeighbors);
                newU = valueOfNeighborhoodCoalition(a1);
            }
            aN.utilityCP = (float)(orginalU.ContributionInNet - newU.ContributionInNet) / orginalU.ContributionInNet+
                (orginalU.ContributionInSubN-newU.ContributionInSubN)/orginalU.ContributionInSubN;
        }


        /// <summary>
        /// 对智能体a1进行拥挤空间调整
        /// </summary>
        /// <param name="a1">被调整智能体</param>
        private void SpaceAdaption(Agent a1)
        {
            List<Location> tempLocList = new List<Location>(0);
            if (a1.list_Neighbors.Count != 0)
            {
                foreach (Neighbor nb in a1.list_Neighbors)
                {
                    float dis = Dist2Agent(a1,nb.agent);
                    //当智能体与其邻居节点nb的距离小于100时，对本节点的位置进行调整
                    //找到离得最近的两个点，在他们的中垂线上找到一个点使得各邻居节点还在内接圆范围内
                    if (dis < 80)
                    {
                        Location loc = new Location();
                        
                    }
                }
            }
        }

        /// <summary>
        /// 计算两个点的中垂线
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        private Location MidPtCompt(Location a1, Location a2)
        {
            Location midPt = new Location();

            return midPt;
        }

        /// <summary>
        /// 某邻居节点对于智能体a的Neighborhood网络联盟的贡献值
        /// </summary>
        /// <param name="a">智能体</param>
        /// <returns>返回智能体a的次邻居utility</returns>
        private agentUtilityInLocalNet UtilityOfSubAgent(Agent a)
        {            
            agentUtilityInLocalNet aNV = new agentUtilityInLocalNet();
            aNV.ContributionInNet = 0;
            aNV.ContributionOfAbility = 0;
            float Nr = 0;
            Nr = (float)(0.5 * this.abilityA.ablityX + 0.5 * this.abilityA.ablityY);
            agentNetValue.ContributionOfAbility = Nr;
            aNV.ContributionOfAbility = Nr;
            float Nn = 0;
            if (a.list_subNeighbors != null && a.list_subNeighbors.Count != 0)
            {
                foreach (Neighbor nb in a.list_subNeighbors)
                {
                    if (!IsNbInNeighbors(nb,a))
                        Nr = (float)(0.5 * nb.agent.abilityA.ablityX + 0.5 * nb.agent.abilityA.ablityY); ;
                    Nn += Nr;
                }
                aNV.ContributionInNet = Nn;
            }
            return aNV;
        }

        /// <summary>
        /// 两个智能体之间邻居集的重叠度
        /// </summary>
        /// <param name="a">智能体a</param>
        /// <param name="b">智能体b</param>
        /// <returns>返回两个智能体的邻居集中重叠的智能体数量</returns>
        private int OverLappingInNeighbors(Agent a, Agent b)
        {
            int overlappingNum = 0;
            foreach (Neighbor n1 in a.list_Neighbors)
            {
                foreach (Neighbor n2 in b.list_Neighbors)
                {
                    if (n2.agent.ID == n1.agent.ID)
                        overlappingNum++;
                }
            }
            return overlappingNum;
        }

        /// <summary>
        /// 计算智能体和其邻居节点之间节点重叠度
        /// </summary>
        /// <param name="a">智能体</param>
        /// <param name="aList">全体智能体集合，用于进行ID匹配</param>
        private void OverLappingCptInNH(Agent a, List<Agent> aList)
        {
            int overlap = 0;
            foreach (Neighbor b in a.list_Neighbors)
            {
                foreach (Agent agent in aList)
                {
                    if (b.agent.ID == agent.ID)
                    {
                        overlap = OverLappingInNeighbors(a, agent);
                        b.overlappingNumInN = overlap;
                    }
                }
            }
        }

        /// <summary>
        /// 计算智能体和其次邻居节点之间节点重叠度
        /// </summary>
        /// <param name="a">智能体</param>
        /// <param name="aList">全体智能体集合，用于进行ID匹配</param>
        private void OverLappingCptInSNH(Agent a, List<Agent> aList)
        {
            int overlap = 0;
            foreach (Neighbor b in a.list_subNeighbors)
            {
                foreach (Agent agent in aList)
                {
                    if (b.agent.ID == agent.ID)
                    {
                        overlap = OverLappingInNeighbors(a, agent);
                        b.overlappingNumInN = overlap;
                    }
                }
            }
        }

        /// <summary>
        /// 根据当前智能体的通信半径，沿着到中心点的位置巡航一个半径
        /// </summary>
        /// <param name="startPt">计算起点</param>
        /// <param name="endPt">计算中点</param>
        /// <returns>返回到中心位置的一个半径的距离点</returns>
        private Location RadioPtCpt(Location startPt, Location endPt)
        {
            Location la = new Location();
            Location temp = new Location();
            //每次前进的间隔
            float interval = 0f;
            interval = (endPt.lat - startPt.lat) / 200;
            float y = 0f;
            //用于存储            
            float disT = Dist2Locations(startPt,endPt);
            LineType lt = new LineType();
            la = endPt;
            float i = startPt.lat ;
            //如果起点到终点的位置在通信范围之内，则移动到中心点
            if (disT <= this.comRadio)
            {
                return endPt;
            }
            else
            {               
                float x;                
                lt = typeOf2PtLine(startPt, la, interval);
                //每次计算之后，la的位置都会更新，因此不断进行线型判断          
                switch (lt)
                {
                    case LineType.NormalLine:
                        //采用相似三角形方法进行坐标计算
                        x = ((comRadio / (2 * disT)) * (endPt.lat - startPt.lat)) + startPt.lat;
                        y = ((comRadio / (2 * disT)) * (endPt.lot - startPt.lot)) + startPt.lot;
                        temp.lat = x;
                        temp.lot = y;
                        la = temp;
                        break;
                    case LineType.VerticalLine:
                        if (startPt.lot <= endPt.lot)
                        {
                            y = startPt.lot + comRadio / 2;
                            temp.lat = startPt.lat;
                            temp.lot = y;
                        }
                        else
                        {
                            y = startPt.lot - comRadio / 2;
                            temp.lat = startPt.lat;
                            temp.lot = y;
                        }
                        la = temp;                    
                        break;
                    case LineType.HorizontalLine:
                        if (startPt.lat <= endPt.lat)
                        {
                            y = temp.lot;
                            temp.lat = startPt.lat + comRadio / 2;
                            temp.lot = y;
                        }
                        else
                        {
                            y = temp.lot;
                            temp.lat = startPt.lat - comRadio / 2;
                            temp.lot = y;
                        }
                        la = temp;
                        break;
                    case LineType.SamePt: //la和起点重合时，继续计算
                        if (startPt.lat + 50 <= 800 )
                        {
                            la.lat = startPt.lat + 50;
                        }
                        else if (startPt.lat + 50 > 800 && startPt.lat - 50 >= 0)
                        {
                            la.lat = startPt.lat - 50;
                        } 
                        break;
                    default:
                        break;
                }              
            }
            return la;
        }

        /// <summary>
        /// 判断两点之间连线的状态，并将该状态进行返回
        /// </summary>
        /// <param name="startPt">起点</param>
        /// <param name="endPt">终点</param>
        /// <returns>返回状态</returns>
        private LineType typeOf2PtLine(Location startPt, Location endPt, float interval)
        {
            LineType mark =  LineType.NormalLine;
            float x = startPt.lat - endPt.lat;
            float y = startPt.lot - endPt.lot;
            ///起点终点的位置接近在一个speed之内
            if (Math.Abs(x) < interval && Math.Abs(y) < interval)
            {
                mark = LineType.SamePt;
            }
            //x轴已经到达，y轴方向继续前进
            else if (Math.Abs(x) < speed && Math.Abs(y) >= speed)
            {
                mark = LineType.VerticalLine;
            }
            //
            else if (Math.Abs(x) >= speed && Math.Abs(y) < speed)
                {
                mark = LineType.HorizontalLine;
            }
            else if (Math.Abs(x) >= speed && Math.Abs(y) >= speed)
            {
                mark = LineType.NormalLine;
            }
            return mark;
        }
        /// <summary>
        /// 根据本智能体的观察对策略集中的策略进行置信度更新
        /// </summary>
        /// <param name="strategyList">策略集</param>
        /// <param name="neighbor_List">用于查找智能体信息的智能体集</param>
        private void BeliefUpDate(List<Strategy> strategyList, List<Agent> neighbor_List)
        {
            ///todo
            if (strategyList != null && strategyList.Count != 0)
            {
                for (int i = 0; i < strategyList.Count; i++)
                {

                    float leP;
                    leP = (float)(((1 - 0.5) - (strategyList[i].item_Neighbor.agent.pTrue)) / ((1 - strategyList[i].item_Neighbor.agent.pTrue) * (1 - 0.5)));
                    float newP;
                    newP = (float)((strategyList[i].item_Neighbor.agent.freedom * 2 - Math.Pow(strategyList[i].item_Neighbor.agent.freedom, 2)) 
                        * (strategyList[i].belief * (1 - 0.5) / (strategyList[i].belief * (1 - 0.5) + (1 - strategyList[i].belief) * (1 - 0.5) * (1 - leP))));
                    Strategy newS = new Strategy();
                    newS = strategyList[i];
                    //对于当前策略对应智能体的置信度计算更新
                    newS.belief = newP;
                    strategyList[i] = newS;
                }
            }
        }


        /// <summary>
        /// 初始化智能体难题的策略集
        /// </summary>
        /// <param name="agent_list">智能体全集，用于对邻居节点的定位和信息获取</param>
        private void StrategyInitial(List<Agent> agent_list)
        {
            ///如果智能体有邻居节点
            if (this.list_Neighbors.Count > 0)
            {
                foreach (Neighbor nb in list_Neighbors)
                {
                    foreach (Agent agent in agent_list)
                        ///当智能体状态为空闲智能体时，才能将其考虑为策略
                        if (nb.agent.ID == agent.ID && agent.statusInCoalition == agentStatusInCoalition.IdleAgent)
                        {
                            if (agent.list_Neighbors.Count != 0 && agent.list_Neighbors != null)
                            {
                                Strategy stra = new Strategy();
                                stra.item_Neighbor = new Neighbor();
                                stra.cost = 0;
                                stra.welfare = 0;
                                stra.belief = 0.5f;
                                foreach (Neighbor b in agent.list_Neighbors)
                                {
                                    stra.item_Neighbor = b;
                                    ///将每一个次邻居节点加入策略集合
                                    if (!strategy_List.Contains(stra))
                                        this.strategy_List.Add(stra);
                                }
                            }
                        }
                }
            }
            ///如果没有邻居节点的话，则采用另外的策略。将策略集合置为零
            else
            {
                strategy_List.Clear();
            }
        }

        /// <summary>
        /// 智能体移动之后，对其策略集进行更新
        /// </summary>
        /// <param name="agent_list">智能体全集</param>
        public void StrategyUpDate(List<Agent> agent_list)
        {
            //如果策略集非空，则进行策略更新，否则则进行初始化
            if (this.strategy_List.Count > 0)
            {
                foreach (Strategy s in strategy_List)
                {
                    ///概率更新，根据观察
                    ///todo
                    Strategy newS = new Strategy();
                    newS = s;
                    foreach (Agent a in agent_list)
                    {
                        ///智能体对应的自由度应当是由其邻居节点数量确定。邻居节点越多，自由度越低。
                        a.freedom = (float)(1 - a.list_Neighbors.Count / maxNeighborCount);
                    }
                }
            }          
        }

        /// <summary>
        /// 每个策略的收益计算，并从中选择出收益最大的策略，并将其标志位标记为true
        /// </summary>
        private void StrategyWelfareCompute(List<Agent> agent_list, List<Neighbor> neighborList)
        {
            Location loc = new Location();
            //Location oldLocation = this.location;
            //是否进行了邻居节点添加的标志位
            bool AddFlag = false;
            ///todo
            //当策略集不为空
            if (this.strategy_List != null && this.strategy_List.Count != 0)
            {
                for(int i =0;i< strategy_List.Count;i++)
                {
                    //将策略点添加到当前节点的邻居节点中，进行最佳位置计算
                    if (!IsAgentInStrategy(neighborList, strategy_List[i]))
                    {
                        neighborList.Add(strategy_List[i].item_Neighbor);
                        AddFlag = true;
                    }
                    loc = OptimalLocationCompt(agent_list, neighborList);
                    //当最小外接圆的半径小于通信半径
                    if (Dist2ID(agent_list, neighborList[0].agent.ID, strategy_List[i].item_Neighbor.agent.ID) < comRadio)
                    {
                        //计算得到最佳位置之后，
                        Strategy s = new Strategy();
                        s = this.strategy_List[i];
                        //将计算得到的位置进行保存
                        s.locationOpt = loc;
                        //计算得到从当前位置移动到目标位置的时间成本
                        float dis = Dist2Locations(loc, this.curtLocattion);
                        s.cost = (dis / speed);
                        ///获得的收益
                        float newV = strategy_List[i].belief * this.valueOfAgentAsNewNeighbor(s.item_Neighbor.agent,agent_list).ContributionInNet;
                        ///收益等于新获得的coalition价值减去移动成本
                        s.welfare = newV - s.cost;
                        this.strategy_List[i] = s;
                        //如果策略在邻居节点集中并且是刚刚添加进去的，则进行移除
                        if (IsAgentInStrategy(neighborList, strategy_List[i]) && AddFlag)
                            neighborList.Remove(strategy_List[i].item_Neighbor);
                    }
                    //如果计算出来的外心到各点的距离超过通信半径。则不纳入策略集
                    else
                    {
                        ;
                    }
                }
                float max = 0;
                max = strategy_List[0].welfare;
                int flag = 0;
                ///选择出收益最大的策略，准备和当前位置进行比较
                for (int i = 1; i < strategy_List.Count; i++)
                {
                    if (strategy_List[i].welfare >= max)
                    {
                        max = strategy_List[i].welfare;
                        flag = i;
                    }
                }
                Strategy s1 = new Strategy();
                s1 = strategy_List[flag];
                s1.flag = true;
                strategy_List[flag] = s1;
            }
           ///当策略集为空，则是进行
        }

        /// <summary>
        /// 判断函数，判断策略s是否在邻居节点集中
        /// </summary>
        /// <param name="neighborList">邻居节点集</param>
        /// <param name="s">策略s</param>
        /// <returns></returns>
        private bool IsAgentInStrategy(List<Neighbor> neighborList, Strategy s)
        {
            bool flag = false;
            foreach (Neighbor b in neighborList)
            {
                if (s.item_Neighbor.agent.ID == b.agent.ID)
                    flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 计算节点A的度（无向图）并返回。同时计算智能体A所在的网络节点的在局域网络结构的熵和局域网络能力的熵
        /// </summary>
        /// <param name="a">智能体a</param>
        /// <returns></returns>
        public int GetDegree(Agent a)
        {
            int degree = 0;
            degree = a.list_Neighbors.Count;            
            return degree;
        }

        /// <summary>
        /// 计算智能体A的DC
        /// </summary>
        /// <param name="a">待计算的智能体A</param>
        /// <returns></returns>
        public CNAMe GetDC(Agent a, int totalNum)
        {
            int LR=0;
            CNAMe newCNA = new CNAMe();
            newCNA.DC = (float)(a.DegreeOfComplex / (totalNum -1));
            newCNA.wDC = (a.abilityA.ablityX + a.abilityA.ablityY) * newCNA.DC;
            foreach(Neighbor nb in a.list_Neighbors)
            {
                LR += nb.agent.LocalCNA.localR;
               }
            newCNA.LocalR = LR/ totalNum;
            return newCNA;
        }

        /// <summary>
        /// 计算智能体A的LDC
        /// </summary>
        /// <param name="a">待计算的智能体A</param>
        /// <returns></returns>
        private float GetLDC(Agent a, int totalNum)
        {
            ///LCD参数是智能体a的全部邻居节点以及次邻居节点的DC平均值
            float LdcA = 0;
            //计算邻居节点的DC
            foreach (Neighbor nb in a.list_Neighbors)
            {
                nb.agent.cnaM = nb.agent.GetDC(nb.agent, totalNum);
                LdcA += nb.agent.cnaM.DC;
            }
            //计算次邻居节点的DC
            foreach (Neighbor nb in a.list_subNeighbors)
            {
                nb.agent.cnaM = nb.agent.GetDC(nb.agent, totalNum);
                LdcA += nb.agent.cnaM.DC;
            }
            //为了防止LDC超过1，对其求平均
            LdcA = LdcA / (a.list_Neighbors.Count + a.list_subNeighbors.Count);
            return LdcA;
        }

        /// <summary>
        /// 计算智能体A的复杂网络Indice值
        /// </summary>
        /// <param name="a">智能体a</param>
        /// <returns></returns>
        private ComplexNetAttrLocal ComplexNetAttributionGet(Agent a, int totalNum)
        {
            ComplexNetAttrLocal newCna = new ComplexNetAttrLocal();
            float totalAbility = 0;
            float SumOfLDC = 0; //所有邻居节点的度的总和
            float SumOfwLDC = 0;//所有邻居节点的度的权重总和
            double Is;  //局域网中智能体所在网络结构的熵
            double Ia;  //局域网中智能体所在网络能力的熵
            int LR; //局域网中智能体所在网络的LocalRank
            LR = a.list_Neighbors.Count + a.list_subNeighbors.Count;
            Is = Ia = 0;
            //更新每一个邻居节点的DC
            foreach (Neighbor nb in a.list_Neighbors)
            {
                nb.agent.cnaM = nb.agent.GetDC(nb.agent, totalNum);
            }
            //更新每一个次邻居节点的CD
            foreach (Neighbor nb in a.list_subNeighbors)
            {
                nb.agent.cnaM = nb.agent.GetDC(nb.agent, totalNum);   
            }
            totalAbility = a.abilityA.ablityX + a.abilityA.ablityY;
            //计算智能体A的本地dc值
            a.cnaM = a.GetDC(a, totalNum);
            //计算LDC和wLDC(LDC是对neighbor集和sub-neighbor集中DC取平均值，)
            foreach(Neighbor nb in a.list_Neighbors)
            {
                //计算当前节点的局域网中的LCD
                SumOfLDC += nb.agent.cnaM.DC;
                SumOfwLDC += nb.agent.cnaM.wDC; 
            }
            foreach (Neighbor nb in a.list_subNeighbors)
            {
                //计算当前节点的局域网中的LCD
                SumOfLDC += nb.agent.cnaM.DC;
                SumOfwLDC += nb.agent.cnaM.wDC;

            }
            foreach (Neighbor nb in a.list_Neighbors)
            {
                //计算当前a智能体的所在局域网络结构的熵
                Is += (-1)*(a.LocalCNA.LDC / SumOfLDC) * Math.Log(a.LocalCNA.LDC / SumOfLDC);
                //计算当前智能体a的所在局域网络能力的熵
                Ia += (-1) * (a.LocalCNA.wLDC / SumOfwLDC) * Math.Log(a.LocalCNA.wLDC / SumOfwLDC);
            }
            newCna.LDC = SumOfLDC;
            newCna.wLDC = SumOfwLDC;
            newCna.eIs = (float)Is;
            newCna.eIa = (float)Ia;
            newCna.localR = LR;
            return newCna;
        }

        /// <summary>
        /// 计算智能体A的复杂网络属性
        /// </summary>
        /// <param name="a">智能体A</param>
        /// <param name="Num">群体总数量</param>
        /// <returns></returns>
        public ComplexNetAttrLocal getLocalCPNAttr(Agent a, int Num)
        {
            ComplexNetAttrLocal newCA = new ComplexNetAttrLocal();
            newCA = a.ComplexNetAttributionGet(a, Num);
            return newCA;
        }

    }
}
