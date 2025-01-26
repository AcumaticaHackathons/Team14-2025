using System;
using PX.Data;
using PX.Objects.IN;

namespace Team14
{
    [Serializable]
    [PXPrimaryGraph(typeof(TDSetupMaint))]
    [PXCacheName("TDSetup")]
    public class TDSetup : PXBqlTable, IBqlTable
    {
        #region ApiKey
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Api Key")]
        public virtual string ApiKey { get; set; }
        public abstract class apiKey : PX.Data.BQL.BqlString.Field<apiKey> { }
        #endregion

        #region ConfigPath
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Config Path")]
        public virtual string ConfigPath { get; set; }
        public abstract class configPath : PX.Data.BQL.BqlString.Field<configPath> { }
        #endregion

        #region ExePath
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Exe Path")]
        public virtual string ExePath { get; set; }
        public abstract class exePath : PX.Data.BQL.BqlString.Field<exePath> { }
        #endregion

        #region WorkingPath
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Working Path")]
        public virtual string WorkingPath { get; set; }
        public abstract class workingPath : PX.Data.BQL.BqlString.Field<workingPath> { }
        #endregion

        #region RenderSize
        [PXDBInt()]
        [PXUIField(DisplayName = "Render Size")]
        public virtual int? RenderSize { get; set; }
        public abstract class renderSize : PX.Data.BQL.BqlInt.Field<renderSize> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
    }
}