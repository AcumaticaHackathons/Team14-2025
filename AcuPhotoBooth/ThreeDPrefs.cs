using System;
using PX.Data;

namespace AcuPhotoBooth
{
  [Serializable]
  [PXCacheName("ThreeDPrefs")]
  public class ThreeDPrefs : PXBqlTable, IBqlTable
  {
    #region ConfigPath
    [PXDBString(1024, InputMask = "")]
    [PXUIField(DisplayName = "Config Path")]
    public virtual string ConfigPath { get; set; }
    public abstract class configPath : PX.Data.BQL.BqlString.Field<configPath> { }
    #endregion

    #region PrinterIP
    [PXDBString(15, InputMask = "")]
    [PXUIField(DisplayName = "Printer IP")]
    public virtual string PrinterIP { get; set; }
    public abstract class printerIP : PX.Data.BQL.BqlString.Field<printerIP> { }
    #endregion

    #region TempPath
    [PXDBString(1024, InputMask = "")]
    [PXUIField(DisplayName = "Temp Path")]
    public virtual string TempPath { get; set; }
    public abstract class tempPath : PX.Data.BQL.BqlString.Field<tempPath> { }
    #endregion

    #region PrusaPath
    [PXDBString(1024, InputMask = "")]
    [PXUIField(DisplayName = "Prusa Path")]
    public virtual string PrusaPath { get; set; }
    public abstract class prusaPath : PX.Data.BQL.BqlString.Field<prusaPath> { }
    #endregion

    #region MaxSize
    [PXDBInt()]
    [PXUIField(DisplayName = "Max Size")]
    public virtual int? MaxSize { get; set; }
    public abstract class maxSize : PX.Data.BQL.BqlInt.Field<maxSize> { }
    #endregion

    #region Awskey
    [PXDBString(100, InputMask = "")]
    [PXUIField(DisplayName = "Awskey")]
    public virtual string Awskey { get; set; }
    public abstract class awskey : PX.Data.BQL.BqlString.Field<awskey> { }
    #endregion

    #region AWSSecret
    [PXDBString(100, InputMask = "")]
    [PXUIField(DisplayName = "AWSSecret")]
    public virtual string AWSSecret { get; set; }
    public abstract class aWSSecret : PX.Data.BQL.BqlString.Field<aWSSecret> { }
    #endregion

    #region AWSRegion
    [PXDBString(50, InputMask = "")]
    [PXUIField(DisplayName = "AWSRegion")]
    public virtual string AWSRegion { get; set; }
    public abstract class aWSRegion : PX.Data.BQL.BqlString.Field<aWSRegion> { }
    #endregion

    #region AWSBucket
    [PXDBString(200, InputMask = "")]
    [PXUIField(DisplayName = "AWSBucket")]
    public virtual string AWSBucket { get; set; }
    public abstract class aWSBucket : PX.Data.BQL.BqlString.Field<aWSBucket> { }
    #endregion
  }
}