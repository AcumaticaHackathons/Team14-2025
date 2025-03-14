using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcuPhotoBooth
{
    public class ThreeDPrefMaint : PXGraph<ThreeDPrefMaint>
    {
        public PXSelect<ThreeDPrefs> Document;

        public PXSave<ThreeDPrefs> Save;
        public PXCancel<ThreeDPrefs> Cancel;
    }
}
