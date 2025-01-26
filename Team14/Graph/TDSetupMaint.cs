using System;
using PX.Data;

namespace Team14
{
    public class TDSetupMaint : PXGraph<TDSetupMaint>
    {

        public PXSave<TDSetup> Save;
        public PXCancel<TDSetup> Cancel;


        public PXSelect<TDSetup> Preferences;

    }
}