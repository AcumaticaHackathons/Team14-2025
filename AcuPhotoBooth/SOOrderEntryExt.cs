using PX.Data;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.SO;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AcuPhotoBooth.StaticObjects;

namespace AcuPhotoBooth
{
    public class SOOrderEntryExt : PXGraphExtension<SOOrderEntry>
    {

        public PXSelect<ThreeDProgress, Where<ThreeDProgress.orderType, Equal<Current<SOOrder.orderType>>, And<ThreeDProgress.orderNbr, Equal<Current<SOOrder.orderNbr>>>>> OrderProgress;
        public PXAction<SOOrder> ProcessImage;
        [PXButton]
        [PXUIField(DisplayName = "Process Image")]
        public IEnumerable processImage(PXAdapter adapter)
        {
            var list = adapter.Get<SOOrder>();
            PXLongOperation.StartOperation(Base, () =>
            {
                foreach (var item in list)
                {
                    {
                        ProcessItem(item);
                    }

                }
            });
            return adapter.Get();
        }
        public static void ProcessItem(SOOrder item)
        {
            var graph = PXGraph.CreateInstance<SOOrderEntry>();
            graph.Document.Current = item;
            var upload = PXGraph.CreateInstance<UploadFileMaintenance>();
            var results = PXNoteAttribute.GetFileNotes(graph.Document.Cache, item);
            foreach (var result in results)
            {
                var info = upload.GetFile(result);
                if (info.OriginalName.ToLower().Contains(".jpg"))
                {
                    var soLine = new SOLine()
                    {
                        OrderType = item.OrderType,
                        OrderNbr = item.OrderNbr,
                        InventoryID = 10322,
                        TranDesc = info.OriginalName,
                    };
                    soLine = graph.Transactions.Insert(soLine);
                    var fileResult = StaticObjects.ProcessFile("", "", 50, new MemoryStream(info.BinData), string.Format("Order{0}-{1}-Line{2}",soLine.OrderType,soLine.OrderNbr , soLine.LineNbr),item.OrderType,item.OrderNbr,info.UID.Value);
                    graph.Actions.PressSave();
                    PXLongOperation.WaitCompletion(graph);
                    ProcessSOLineItem(item, soLine, info.UID.Value, fileResult);
                    if (upload.IsDirty)
                        upload.Actions.PressSave();
                    graph.Actions.PressSave();
                }
            }
        }
        public static void ProcessSOLineItem(SOOrder item, SOLine line, Guid FileID, FileProcessResult fileResult)
        {
            var graph = PXGraph.CreateInstance<SOOrderEntry>();
            graph.Document.Current = item;
            graph.Transactions.Current = line;
            var upGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
            var fmGrpah = PXGraph.CreateInstance<UploadFileMaintenance>();
            var oldfile=upGraph.GetFile(FileID);
            //delete orig file from salesorder
            UploadFileMaintenance.DeleteFile (FileID);
            //attach orig file to SO Line
            upGraph.SaveFile(oldfile,FileExistsAction.CreateVersion);
            PXNoteAttribute.AttachFile(graph.Transactions.Cache, graph.Transactions.Current, oldfile);
            if (fileResult?.PreviewFile != null)
            {
                //attatch Preview to SOLine
                var file = new PX.SM.FileInfo("ModelPreview.jpg", "", new BinaryReader(fileResult.PreviewFile).ReadBytes((int)fileResult.PreviewFile.Length));
                upGraph.SaveFile(file, FileExistsAction.CreateVersion);
                PXNoteAttribute.AttachFile(graph.Transactions.Cache, graph.Transactions.Current, file);
            }
            if (fileResult.GCodeFile != null)
            {
                //attatch gcode to SOLine
                var file = new PX.SM.FileInfo("Model.gcode", "", new BinaryReader(fileResult.GCodeFile).ReadBytes((int)fileResult.GCodeFile.Length));
                upGraph.SaveFile(file, FileExistsAction.CreateVersion);
                PXNoteAttribute.AttachFile(graph.Transactions.Cache, graph.Transactions.Current, file);
            }
            if (fileResult?.STLFile != null)
            {
                //attach STL to SO Line
                var file = new PX.SM.FileInfo("Model.STL", "", new BinaryReader(fileResult.STLFile).ReadBytes((int)fileResult.STLFile.Length));
                upGraph.SaveFile(file, FileExistsAction.CreateVersion);
                PXNoteAttribute.AttachFile(graph.Transactions.Cache, graph.Transactions.Current, file);
            }
            if (upGraph.IsDirty)
                upGraph.Actions.PressSave();
            graph.Actions.PressSave();
        }

    }
}
