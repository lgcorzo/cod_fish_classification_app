using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HalconDotNet;

namespace Camera
{
    public class CameraCalibration
    {
        public HTuple CameraParameters;
        public HPose Pose;
        public HTuple OtherParameters;

        HImage CalibratedMap;

        public CameraCalibration()
        {
        }

        public CameraCalibration(HTuple parameters, HPose pose, HTuple oParam)
        {
            try
            {
                CameraParameters = parameters;
                Pose = pose;
                OtherParameters = oParam;

                this.CalibratedMap = new HImage();
                this.CalibratedMap.GenImageToWorldPlaneMap(CameraParameters, Pose, OtherParameters[0].I, OtherParameters[1].I, OtherParameters[2].I, OtherParameters[3].I, (HTuple)OtherParameters[4], "bilinear");
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public double GetScaleMm()
        {
            try
            {
                return OtherParameters[4].D * 1000;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void LoadCamCalibration(string path, string camName)
        {
            try
            {
                CameraParameters = new HTuple();
                HOperatorSet.ReadTuple(path + "\\" + camName + "_CameraParameters", out CameraParameters);

                HTuple PoseTuple = new HTuple();
                HOperatorSet.ReadPose(path + "\\" + camName + "_Pose", out PoseTuple);
                Pose = new HPose(PoseTuple);

                OtherParameters = new HTuple();
                HOperatorSet.ReadTuple(path + "\\" + camName + "_OtherParameters", out OtherParameters);

                this.CalibratedMap = new HImage();
                this.CalibratedMap.GenImageToWorldPlaneMap(CameraParameters, Pose, OtherParameters[0].I, OtherParameters[1].I, OtherParameters[2].I, OtherParameters[3].I, (HTuple)OtherParameters[4], "bilinear");
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public HImage ImageCalibrated(HImage image, bool mapped)
        {
            try
            {
                if (mapped)
                    return image.MapImage(this.CalibratedMap);
                else
                    return image.ImageToWorldPlane(CameraParameters, Pose, OtherParameters[0].I, OtherParameters[1].I, (HTuple)OtherParameters[4], "bilinear");

                //return image;
            }
            catch 
            {
                return new HImage();
            }
        }

        public void PointsToWorldPlane(HTuple Row, HTuple Col, out HTuple X, out HTuple Y, double alturaPlano)
        {
            try
            {
                HPose poseAct;
                if (alturaPlano != 0)
                {
                    poseAct = Pose.SetOriginPose(0, 0, alturaPlano);
                }
                else
                {
                    poseAct = Pose;
                }

                HOperatorSet.ImagePointsToWorldPlane(CameraParameters, poseAct, Row, Col, "mm", out X, out Y);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public string[] ToStringArray()
        {
            try
            {
                string[] paramsStr = new string[3];

                paramsStr[0] = CameraParameters.ToString();
                paramsStr[1] = Pose.ToString();
                paramsStr[2] = OtherParameters.ToString();
                return paramsStr;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

    }
}
