using System;
using System.Collections.Generic;

public class QAIML
{
    //Simple AIML: Input (0) >> Hidden (Neutral) (L-1) >> Output (L) >> Desired

    public QAIML()
    {
        SetReset();
    }

    //Number of Layer in Neural (Input - Hidden - Output) (*)
    private int m_LayerCount = 2;

    //List of Neural in Layer (Include Input - Hidden - Output) (*)
    private List<List<float>> m_Activation;

    //Number of Neural in Layer (Include Input - Hidden - Output) (0)
    private List<int> m_NeuralCount;

    //List of Weight[L][L-1] Layer L (Include Hidden - Output) (-1)
    private List<List<List<float>>> m_Weight;

    //List of Bias Layer (Include Input - Hidden) (-1)
    private List<float> m_Bias;

    //List of Sum Layer (Include Hidden - Output) (-1)
    private List<List<float>> m_Sum;

    //List of Error Layer (Include Hidden - Output) (-1)
    private List<List<float>> m_Error;

    //List of Neural Output wanted
    private List<float> m_Desired;

    /// <summary>
    /// Reset Neural Network
    /// </summary>
    public void SetReset()
    {
        m_NeuralCount = new List<int>();
        m_Activation = new List<List<float>>();
        m_Weight = new List<List<List<float>>>();
        m_Bias = new List<float>();
        m_Sum = new List<List<float>>();
        m_Error = new List<List<float>>();
        m_Desired = new List<float>();
    }

    #region ==================================== Set - Should call in order

    #region ------------------------------------ Set Primary

    #region New Version

    /// <summary>
    /// Start create new Neural Network
    /// </summary>
    /// <param name="RandomNumber">If "True", Weight & Bias will gain random value</param>
    public void SetNeuralNetworkCreate(params int[] NeutralCountEachLayer)
    {
        //Neutral Network Generate
        m_LayerCount = NeutralCountEachLayer.Length;
        for (int lay = 0; lay < m_LayerCount; lay++)
        {
            m_NeuralCount.Add(NeutralCountEachLayer[lay]);
        }

        //Activation
        m_Activation = new List<List<float>>();
        for (int lay = 0; lay < m_LayerCount; lay++)
        {
            m_Activation.Add(new List<float> { });
            for (int neu = 0; neu < m_NeuralCount[lay]; neu++)
            {
                m_Activation[lay].Add(0);
            }
        }

        //Weight
        m_Weight = new List<List<List<float>>>();
        for (int lay = 0; lay < m_LayerCount - 1; lay++)
        {
            m_Weight.Add(new List<List<float>> { });
            for (int neuY = 0; neuY < m_NeuralCount[lay + 1]; neuY++)
            {
                m_Weight[lay].Add(new List<float> { });
                for (int neuX = 0; neuX < m_NeuralCount[lay]; neuX++)
                {
                    m_Weight[lay][neuY].Add(0.0f);
                }
            }
        }

        //Bias
        m_Bias = new List<float>();
        for (int lay = 0; lay < m_LayerCount - 1; lay++)
        {
            m_Bias.Add(0.0f);
        }

        //Sum
        m_Sum = new List<List<float>>();
        for (int lay = 1; lay < m_LayerCount; lay++)
        {
            m_Sum.Add(new List<float> { });
            for (int neu = 0; neu < m_NeuralCount[lay]; neu++)
            {
                m_Sum[lay - 1].Add(0.0f);
            }
        }

        //Error
        m_Error = new List<List<float>>();
        for (int lay = 0; lay < m_LayerCount - 1; lay++)
        {
            m_Error.Add(new List<float> { });
            for (int neu = 0; neu < m_NeuralCount[lay + 1]; neu++)
            {
                m_Error[lay].Add(0.0f);
            }
        }

        //Desired
        m_Desired = new List<float>();
        for (int neu = 0; neu < m_NeuralCount[m_LayerCount - 1]; neu++)
        {
            m_Desired.Add(0.0f);
        }
    }

    #endregion

    #region Old Version

    /// <summary>
    /// Set Number of Layer
    /// </summary>
    /// <param name="LayerCount"></param>
    public void SetLayerCount(int LayerCount)
    {
        QPlayerPrefs.SetValue("LC", (LayerCount < 0) ? 2 : LayerCount);
    }

    /// <summary>
    /// Set new Number of Neutral count of each Neural Layer
    /// </summary>
    /// <param name="Layer"></param>
    /// <param name="NeuralCount"></param>
    public void SetNeuralCount(int Layer, int NeuralCount)
    {
        if (Layer >= 0)
        {
            QPlayerPrefs.SetValue("NC_" + Layer.ToString(), (NeuralCount > 0) ? NeuralCount : 0);
        }
    }

    /// <summary>
    /// Start create new Neural Network
    /// </summary>
    /// <param name="RandomNumber">If "True", Weight & Bias will gain random value</param>
    public void SetNeuralNetworkCreate(bool RandomNumber = false)
    {
        //LayerCount
        m_LayerCount = QPlayerPrefs.GetValueInt("LC");

        //NeuralCount
        m_NeuralCount = new List<int>();
        for (int lay = 0; lay < m_LayerCount; lay++)
        {
            m_NeuralCount.Add(QPlayerPrefs.GetValueInt("NC_" + lay.ToString()));
        }

        //Activation
        m_Activation = new List<List<float>>();
        for (int lay = 0; lay < m_LayerCount; lay++)
        {
            m_Activation.Add(new List<float> { });
            for (int neu = 0; neu < m_NeuralCount[lay]; neu++)
            {
                m_Activation[lay].Add(0);
            }
        }

        //Weight
        m_Weight = new List<List<List<float>>>();
        for (int lay = 0; lay < m_LayerCount - 1; lay++)
        {
            m_Weight.Add(new List<List<float>> { });
            for (int neuY = 0; neuY < m_NeuralCount[lay + 1]; neuY++)
            {
                m_Weight[lay].Add(new List<float> { });
                for (int neuX = 0; neuX < m_NeuralCount[lay]; neuX++)
                {
                    if (RandomNumber)
                    {
                        System.Random Rand = new System.Random();
                        float Value = (float)((m_LayerCount * Rand.Next(1, 500) + m_NeuralCount[lay] * Rand.Next(500, 1000) + neuX * Rand.Next(100, 200) + neuY * Rand.Next(200, 300) + lay * Rand.Next(300, 400)) * Rand.Next(1, 50) / 100000.0) / 100.0f;
                        m_Weight[lay][neuY].Add(Value);
                    }
                    else
                    {
                        m_Weight[lay][neuY].Add(0.0f);
                    }

                }
            }
        }

        //Bias
        m_Bias = new List<float>();
        for (int lay = 0; lay < m_LayerCount - 1; lay++)
        {
            if (RandomNumber)
            {
                System.Random Rand = new System.Random();
                float Value = 0;
                m_Bias.Add(Value);
            }
            else
            {
                m_Bias.Add(0.0f);
            }
        }

        //Sum
        m_Sum = new List<List<float>>();
        for (int lay = 1; lay < m_LayerCount; lay++)
        {
            m_Sum.Add(new List<float> { });
            for (int neu = 0; neu < m_NeuralCount[lay]; neu++)
            {
                m_Sum[lay - 1].Add(0.0f);
            }
        }

        //Error
        m_Error = new List<List<float>>();
        for (int lay = 0; lay < m_LayerCount - 1; lay++)
        {
            m_Error.Add(new List<float> { });
            for (int neu = 0; neu < m_NeuralCount[lay + 1]; neu++)
            {
                m_Error[lay].Add(0.0f);
            }
        }

        //Desired
        m_Desired = new List<float>();
        for (int neu = 0; neu < m_NeuralCount[m_LayerCount - 1]; neu++)
        {
            m_Desired.Add(0.0f);
        }
    }

    #endregion

    #endregion

    #region ------------------------------------ Set Runtime

    #region Set Input - Start before First Result and after every Result and Desired

    /// <summary>
    /// Set new Input
    /// </summary>
    /// <param name="InputNew"></param>
    /// <param name="InputValue"></param>
    public void SetInputLayerInput(int InputNew, float InputValue)
    {
        if (InputNew >= 0 && InputNew < m_NeuralCount[0])
        {
            m_Activation[0][InputNew] = InputValue;
        }
    }

    /// <summary>
    /// Set new Input
    /// </summary>
    /// <param name="mInputList"></param>
    public void SetInputLayerInput(List<float> InputList)
    {
        if (InputList == null)
        {
            return;
        }

        if (m_Activation[0].Count == InputList.Count)
        {
            m_Activation[0] = InputList;
        }
        else
        {
            for (int i = 0; i < InputList.Count; i++)
            {
                m_Activation[0][i] = InputList[i];
            }
        }
    }

    /// <summary>
    /// Set Input
    /// </summary>
    /// <param name="InputList"></param>
    /// <param name="From"></param>
    public void SetInputLayerInput(List<float> InputList, int From)
    {
        if (InputList == null)
        {
            return;
        }

        for (int i = 0; i < InputList.Count; i++)
        {
            m_Activation[0][i + From] = InputList[i];
        }
    }

    #endregion

    #region Set Desired - After First Result and after every Result, AI will learn from Desired

    /// <summary>
    /// Set new Output Desired
    /// </summary>
    /// <param name="NeutralDesired"></param>
    /// <param name="ValueDesired"></param>
    public void SetInputDesired(int NeutralDesired, float ValueDesired)
    {
        if (NeutralDesired >= 0 && NeutralDesired < m_NeuralCount[m_LayerCount - 1])
        {
            m_Desired[NeutralDesired] = ValueDesired;
        }
    }

    #endregion

    /// <summary>
    /// Set new Bias in Layer
    /// </summary>
    /// <param name="Layer"></param>
    /// <param name="Bias"></param>
    public void SetInputBias(int Layer, float Bias)
    {
        if (Layer < m_LayerCount && Layer >= 0)
        {
            m_Bias[Layer] = Bias;
        }
    }

    /// <summary>
    /// Set new Weight with X (L-1) << Y (L)
    /// </summary>
    /// <param name="Layer">L</param>
    /// <param name="NeuralY">Y (L)</param>
    /// <param name="NeuralX">X (L-1)</param>
    /// <param name="Weight"></param>
    public void SetInputWeight(int Layer, int NeuralY, int NeuralX, float Weight)
    {
        if (Layer < m_LayerCount - 1 && Layer >= 0)
        {
            m_Weight[Layer][NeuralY][NeuralX] = Weight;
        }
    }

    #endregion

    #endregion

    #region ==================================== Get

    /// <summary>
    /// Get Number of Layer
    /// </summary>
    /// <returns></returns>
    public int GetOutputLayerCount()
    {
        return m_LayerCount;
    }

    /// <summary>
    /// Get Number of Neural of Layer
    /// </summary>
    /// <param name="Layer"></param>
    /// <returns></returns>
    public int GetOutputNeuralCount(int Layer)
    {
        return m_NeuralCount[Layer];
    }

    /// <summary>
    /// Get List of Neural of Input
    /// </summary>
    /// <returns></returns>
    public List<float> GetOutputLayerInput()
    {
        return m_Activation[0];
    }

    /// <summary>
    /// Get Neural from Input
    /// </summary>
    /// <param name="Neural"></param>
    /// <returns></returns>
    public float GetOutputLayer(int Neural)
    {
        return m_Activation[0][Neural];
    }

    /// <summary>
    /// Get Output Desired
    /// </summary>
    /// <returns></returns>
    public List<float> GetOutputDesired()
    {
        return m_Desired;
    }

    /// <summary>
    /// Get List of Neural of Output
    /// </summary>
    /// <returns></returns>
    public List<float> GetOutputLayerOutput()
    {
        return m_Activation[m_LayerCount - 1];
    }

    /// <summary>
    /// Get Neural of Output
    /// </summary>
    /// <param name="Neural"></param>
    /// <returns></returns>
    public float GetOutputLayerOutput(int Neural)
    {
        return m_Activation[m_LayerCount - 1][Neural];
    }

    #endregion

    #region ==================================== File

    /// <summary>
    /// Check AIML File Exist
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public bool GetFileExist(string Path)
    {
        return QPath.GetPathFileExist(Path);
    }

    /// <summary>
    /// Save Current AIML Data to File work on this Script
    /// </summary>
    /// <param name="Path"></param>
    public void SetFileSave(string Path)
    {
        QDataFile FileIO = new QDataFile();

        FileIO.SetWriteAdd("LayerCount:");
        FileIO.SetWriteAdd(m_LayerCount);

        FileIO.SetWriteAdd("NeuralCount:");
        for (int lay = 0; lay < m_LayerCount; lay++)
        {
            FileIO.SetWriteAdd(m_NeuralCount[lay]);
        }

        FileIO.SetWriteAdd("Bias:");
        for (int lay = 0; lay < m_LayerCount - 1; lay++)
        {
            FileIO.SetWriteAdd(m_Bias[lay]);
        }

        FileIO.SetWriteAdd("Weight:");
        for (int lay = 0; lay < m_LayerCount - 1; lay++)
        {
            //Check from Layer (L-1)
            for (int neuY = 0; neuY < m_NeuralCount[lay + 1]; neuY++)
            {
                //Check from Neutral Y of Layer (L)
                for (int neuX = 0; neuX < m_NeuralCount[lay]; neuX++)
                {
                    //Check from Neutral X of Layer (L-1)
                    FileIO.SetWriteAdd(m_Weight[lay][neuY][neuX]);
                }
            }
        }

        FileIO.SetWriteStart(Path);
    }

    /// <summary>
    /// Read AIML Data from File work on this Script
    /// </summary>
    /// <param name="Path"></param>
    public void SetFileOpen(string Path)
    {
        QDataFile FileIO = new QDataFile();

        FileIO.SetReadStart(Path);

        string FileIORead;

        FileIORead = FileIO.GetReadAutoString();
        int LayerCount = FileIO.GetReadAutoInt();
        SetLayerCount(LayerCount);

        FileIORead = FileIO.GetReadAutoString();
        for (int lay = 0; lay < LayerCount; lay++)
        {
            SetNeuralCount(lay, FileIO.GetReadAutoInt());
        }

        SetNeuralNetworkCreate(false);

        FileIORead = FileIO.GetReadAutoString();
        for (int lay = 0; lay < LayerCount - 1; lay++)
        {
            SetInputBias(lay, FileIO.GetReadAutoFloat());
        }

        FileIORead = FileIO.GetReadAutoString();
        for (int lay = 0; lay < LayerCount - 1; lay++)
        {
            //Check from Layer (L-1)
            for (int neuY = 0; neuY < m_NeuralCount[lay + 1]; neuY++)
            {
                //Check from Neutral Y of Layer (L)
                for (int neuX = 0; neuX < m_NeuralCount[lay]; neuX++)
                {
                    //Check from Neutral X of Layer (L-1)
                    SetInputWeight(lay, neuY, neuX, FileIO.GetReadAutoFloat());
                }
            }
        }
    }

    #endregion

    #region ==================================== FeedForward

    /// <summary>
    /// Caculate Sum of between two Layer X (L-1) >> Y (L)
    /// </summary>
    /// <param name="Layer"></param>
    private void SetFeedForwardSum(int Layer)
    {
        for (int neuY = 0; neuY < m_NeuralCount[Layer]; neuY++)
        {
            //Check Layer Y (L)

            //Sum = Weight * m_Activation(L-1) + Bias
            m_Sum[Layer - 1][neuY] = m_Bias[Layer - 1];

            for (int neuX = 0; neuX < m_NeuralCount[Layer - 1]; neuX++)
            {
                //Check Layer X (L-1)
                m_Sum[Layer - 1][neuY] += m_Weight[Layer - 1][neuY][neuX] * m_Activation[Layer - 1][neuX];
            }
        }
    }

    /// <summary>
    /// Caculate Sigmoid
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public float GetFeedForwardSigmoidSingle(float Value)
    {
        return (float)1.0 / ((float)1.0 + (float)Math.Exp(-Value));
    }

    /// <summary>
    /// Caculate Sigmoid from Sum of Layer Y (L)
    /// </summary>
    /// <param name="Layer"></param>
    private void SetFeedForwardSigmoid(int Layer)
    {
        for (int neuY = 0; neuY < m_NeuralCount[Layer]; neuY++)
        {
            //Check Layer Y (L)

            //m_Activation(L) = Sigmoid(Sum)
            m_Activation[Layer][neuY] = GetFeedForwardSigmoidSingle(m_Sum[Layer - 1][neuY]);
        }
    }

    /// <summary>
    /// Active FeedForward
    /// </summary>
    public void SetFeedForward()
    {
        for (int lay = 1; lay < m_LayerCount; lay++)
        {
            SetFeedForwardSum(lay);
            SetFeedForwardSigmoid(lay);
        }
    }

    #endregion

    #region ==================================== BackPropagation

    /// <summary>
    /// Caculate Error between Layer Output >> Desired
    /// </summary>
    private void SetBackPropagationErrorOuput()
    {
        int layerY = m_LayerCount - 1;
        for (int neuY = 0; neuY < m_NeuralCount[layerY]; neuY++)
        {
            //Check Layer Y (L) with Desired
            m_Error[layerY - 1][neuY] = -(m_Desired[neuY] - m_Activation[layerY][neuY]);
        }
    }

    /// <summary>
    /// Caculate Sigmoid
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public float GetBackPropagationSigmoidSingle(float Value)
    {
        float Sigmoid = GetFeedForwardSigmoidSingle(Value);
        return Sigmoid * ((float)1.0 - Sigmoid);
    }

    /// <summary>
    /// Caculate Error between Layer Y (L) >> Layer Z (L+1)
    /// </summary>
    /// <param name="Layer"></param>
    private void SetBackPropagationErrorHidden(int Layer)
    {
        int layerZ = Layer + 1;
        int layerY = Layer;
        for (int neuY = 0; neuY < m_NeuralCount[layerY]; neuY++)
        {
            //Check Layer Y (L) with Layer Z (L+1)
            m_Error[layerY - 1][neuY] = 0;

            for (int neuZ = 0; neuZ < m_NeuralCount[layerZ]; neuZ++)
            {
                //Check Layer Y (L) with Layer Z (L+1)
                m_Error[layerY - 1][neuY] += m_Error[layerZ - 1][neuZ] * GetBackPropagationSigmoidSingle(m_Sum[layerZ - 1][neuZ]) * m_Weight[layerZ - 1][neuZ][neuY];
            }
        }
    }

    /// <summary>
    /// Set Weight Layer
    /// </summary>
    /// <param name="Layer"></param>
    private void SetBackPropagationUpdate(int Layer)
    {
        //Layer Output
        int layerX = Layer - 1;
        int layerY = Layer;
        for (int neuX = 0; neuX < m_NeuralCount[layerX]; neuX++)
        {
            //Check Layer X (L-1) >> Layer Y (L)
            for (int neuY = 0; neuY < m_NeuralCount[layerY]; neuY++)
            {
                //Check Layer X (L-1) >> Layer Y (L)
                m_Weight[layerY - 1][neuY][neuX] -= (float)0.5 * (m_Error[layerY - 1][neuY] * GetBackPropagationSigmoidSingle(m_Sum[layerY - 1][neuY]) * m_Activation[layerX][neuX]);
            }
        }
    }

    /// <summary>
    /// Active BackPropagation
    /// </summary>
    public void SetBackPropagation()
    {
        for (int lay = m_LayerCount - 1; lay > 0; lay--)
        {
            //Check Layer X (L-1) >> Layer Y (L) >> Layer Z (L+1)

            //Caculate Error
            if (lay == m_LayerCount - 1)
            {
                SetBackPropagationErrorOuput();
            }
            else
            {
                SetBackPropagationErrorHidden(lay);
            }
        }

        for (int lay = m_LayerCount - 1; lay > 0; lay--)
        {
            //Check Layer X (L-1) >> Layer Y (L) >> Layer Z (L+1)

            //Set
            SetBackPropagationUpdate(lay);
        }
    }

    #endregion

    #region ==================================== Delay

    /// <summary>
    /// Delay Time
    /// </summary>
    private int m_BrainDelayTime = 3;
    private int m_BrainDelayTimeCurrent = 0;

    /// <summary>
    /// Set Delay Time
    /// </summary>
    /// <param name="Value"></param>
    public void SetBrainDelayTime(int Value)
    {
        m_BrainDelayTime = Value;
    }

    /// <summary>
    /// Check Delay Time
    /// </summary>
    /// <returns>Will return "True" if Delay Time = 0</returns>
    public bool GetBrainDelayTimeValue()
    {
        if (m_BrainDelayTimeCurrent > 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Check Delay Time & Continue work on Delay Time
    /// </summary>
    /// <returns>Will return "True" if Delay Time = 0</returns>
    public bool GetBrainDelayTimeOver()
    {
        if (m_BrainDelayTimeCurrent > 0)
        {
            m_BrainDelayTimeCurrent -= 1;
            return false;
        }
        m_BrainDelayTimeCurrent = m_BrainDelayTime;
        return true;
    }

    #endregion
}
