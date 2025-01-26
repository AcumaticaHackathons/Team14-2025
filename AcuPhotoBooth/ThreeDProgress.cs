using System;
using PX.Data;

namespace AcuPhotoBooth
{
  [Serializable]
  [PXCacheName("ThreeDProgress")]
  public class ThreeDProgress : PXBqlTable, IBqlTable
  { 
    #region OrderType
    [PXDBString(2, IsKey = true, InputMask = "")]
    [PXUIField(DisplayName = "Order Type")]
    public virtual string OrderType { get; set; }
    public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
    #endregion

    #region OrderNbr
    [PXDBString(30, IsKey = true, InputMask = "")]
    [PXUIField(DisplayName = "Order Nbr")]
    public virtual string OrderNbr { get; set; }
    public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
    #endregion

    #region Fileid
    [PXDBGuid(IsKey = true)]
    [PXUIField(DisplayName = "Fileid")]
    public virtual Guid? Fileid { get; set; }
    public abstract class fileid : PX.Data.BQL.BqlGuid.Field<fileid> { }
    #endregion

    #region Progress1
    [PXDBInt()]
    [PXUIField(DisplayName = "Progress1")]
    public virtual int? Progress1 { get; set; }
    public abstract class progress1 : PX.Data.BQL.BqlInt.Field<progress1> { }
    #endregion

    #region Progress2
    [PXDBInt()]
    [PXUIField(DisplayName = "Progress2")]
    public virtual int? Progress2 { get; set; }
    public abstract class progress2 : PX.Data.BQL.BqlInt.Field<progress2> { }
    #endregion

    #region SentToPrinter
    [PXDBBool()]
    [PXUIField(DisplayName = "Sent To Printer")]
    public virtual bool? SentToPrinter { get; set; }
    public abstract class sentToPrinter : PX.Data.BQL.BqlBool.Field<sentToPrinter> { }
    #endregion
  }
}