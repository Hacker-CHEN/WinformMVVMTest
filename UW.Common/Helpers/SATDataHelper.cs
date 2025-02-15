using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public class SATDataHelper
    {
        public static double StDev(double[] arrData) //计算标准偏差
        {
            double xSum = arrData.Sum();
            double xAvg = 0D;
            double sSum = 0D;
            double tmpStDev = 0D;
            int arrNum = arrData.Length;

            xAvg = xSum / arrNum;
            for (int j = 0; j < arrNum; j++)
            {
                sSum += ((arrData[j] - xAvg) * (arrData[j] - xAvg));
            }
            tmpStDev = System.Convert.ToSingle(Math.Sqrt(sSum / (arrNum - 1)).ToString());
            return tmpStDev;
        }

        public static double Cp(double UpperLimit, double LowerLimit, double StDev)//计算cp
        {
            double tmpV = 0D;
            tmpV = UpperLimit - LowerLimit;
            return Math.Abs(tmpV / (6 * StDev));
        }

        public static double Avage(double[] arrData)    //计算平均值
        {
            return arrData.Average();
        }

        public static double Max(double[] arrData)   //计算最大值
        {
            return arrData.Max();
        }

        public static double Min(double[] arrData)  //计算最小值
        {
            return arrData.Min();
        }

        public static double CpkU(double UpperLimit, double Avage, double StDev)//计算CpkU
        {
            double tmpV = 0D;
            tmpV = UpperLimit - Avage;
            return tmpV / (3 * StDev);
        }

        public static double CpkL(double LowerLimit, double Avage, double StDev) //计算CpkL
        {
            double tmpV = 0D;
            tmpV = Avage - LowerLimit;
            return tmpV / (3 * StDev);
        }

        public static double Cpk(double CpkU, double CpkL)  //计算Cpk
        {
            return Math.Abs(Math.Min(CpkU, CpkL));
        }

        public static double getR_value(double[] k_valuesTOO)
        {
            double min = k_valuesTOO[0];
            double max = k_valuesTOO[0];
            for (int i = 0; i < k_valuesTOO.Length; i++)
            {
                if (k_valuesTOO[i] < min)
                {
                    min = k_valuesTOO[i];
                }
                if (k_valuesTOO[i] > max)
                {
                    max = k_valuesTOO[i];
                }
            }
            return max - min;
        }

        public static double getCPK(double[] k, double UpperLimit, double LowerLimit) //获取CPK值
        {
            if (k.Length <= 1 || UpperLimit <= LowerLimit)
            {
                return -1;
            }
            double cpk = Cpk(CpkU(UpperLimit, Avage(k), StDev(k)), CpkL(LowerLimit, Avage(k), StDev(k)));
            return cpk;
        }

        public static double getCP(double[] k, double UpperLimit, double LowerLimit)
        {
            return Cp(UpperLimit, LowerLimit, StDev(k));
        }
    }
}
