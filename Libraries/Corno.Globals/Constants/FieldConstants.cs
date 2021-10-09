namespace Corno.Globals.Constants
{
    public static class FieldConstants
    {
        public const string CornoContext = "CornoContext";

        // All Standard
        public const string CompanyId = "CompanyId";
        public const string FinancialYearId = "FinancialYearId";
        public const string FinancialYear = "FinancialYear";
        public const string SerialNo = "SerialNo";
        public const string Code = "Code";
        public const string Id = "Id";
        public const string Name = "Name";
        public const string Description = "Description";
        public const string Status = "Status";

        public const string PendingQuantity = "PendingQuantity";
        public const string SystemWeight = "SystemWeight";
        public const string ActualWeight = "ActualWeight";
        public const string Variance = "Variance";
        public const string BinNo = "BinNo";
        public const string BatchCode = "BatchCode";
        public const string ScanDate = "ScanDate";


        // All Standard - User Related
        public const string UserId = "UserId";
        public const string UserName = "UserName";
        public const string CreatedBy = "CreatedBy";
        public const string CreatedDate = "CreatedDate";
        public const string ModifiedBy = "ModifiedBy";
        public const string ModifiedDate = "ModifiedDate";
        public const string DeletedBy = "DeletedBy";
        public const string DeletedDate = "DeletedDate";

        // Item Related
        public const string ItemCode = "ItemCode";
        public const string ParentItemCode = "ParentItemCode";
        public const string ItemName = "ItemName";
        public const string MachineType = "Machine Type";
        public const string MachineTypeId = "MachineTypeId";
        public const string ProductType = "Product Type";
        public const string ProductFamily = "Product Family";
        public const string Family = "Family";
        public const string ProductTypeId = "ProductTypeId";
        public const string ProductFamilyId = "ProductFamilyId";
        public const string ProductCategoryId = "ProductCategoryId";
        public const string ProductCategory = "Product Category";
        public const string ConsumableType = "Consumable Type";
        public const string ConsumableTypeId = "ConsumableTypeId";
        public const string Machine = "Machine";
        public const string MachineId = "MachineId";
        public const string SubAssembly = "SubAssembly";
        public const string SubAssemblyId = "SubAssemblyId";
        public const string Cell = "Cell";
        public const string CellId = "CellId";
        public const string EmployeeType = "Employee Type";
        public const string EmployeeTypeId = "EmployeeTypeId";
        public const string Quantity = "Quantity";
        public const string PlannedQuantity = "PlannedQuantity";
        public const string ActualQuantity = "ActualQuantity";
        public const string Rate = "Rate";
        public const string Amount = "Amount";
        public const string Cgst = "Cgst";
        public const string Sgst = "Sgst";
        public const string Igst = "Igst";
        public const string Hsn = "Hsn";
        public const string BomQuantity = "BomQuantity";
        public const string StandardWeight = "StandardWeight";
        public const string ScannedQuantity = "ScannedQuantity";
        public const string ComponentId = "ComponentId";
        public const string Component = "Component";
        public const string ProductId = "ProductId";
        public const string Product = "Product";
        public const string SkuId = "SkuId";
        public const string Sku = "Sku";
        public const string ReasonCodeId = "ReasonCodeId";
        public const string ReasonCode = "ReasonCode";

        public const string Compound = "Compound";
        public const string CompoundId = "CompoundId";
        public const string StationId = "StationId";
        public const string Station = "Station";
        public const string Phr = "Phr";
        public const string Multiple = "Multiple";
        public const string Divide = "Divide";
        public const string Weight = "Weight";

        public const string Ok = "OK";
        public const string Rework = "Rework";
        public const string Process = "Process";
        public const string Recut = "Recut";

        public const string LocationId = "LocationId";
        public const string Location = "Location";
        public const string LineId = "LineId";
        public const string Line = "Line";
        public const string PackingLineId = "PackingLineId";
        public const string PackingLine = "LinePackingLine";
        public static string ManDays = "ManDays";
        public static string PackingManPower = "PackingManPower";
        public static string UnitsPerHour = "UnitsPerHour";

        // Departments 
        public const string AttendanceDate = "AttendanceDate";
        public const string ManPowerAllocationDate = "ManPowerAllocationDate";
        public const string ProductionDate = "ProductionDate";
        public const string ScrapDate = "ScrapDate";
        public const string ConsumableDate = "ConsumableDate";
        public const string DepartmentId = "DepartmentId";

        // General
        public const string Company = "Company";
        public const string Customer = "Customer";
        public const string CustomerId = "CustomerId";
        public const string Supplier = "Supplier";
        public const string SupplierId = "SupplierId";

        public const string Price1 = "Price1";
        public const string Price2 = "Price2";
        public const string Price3 = "Price3";

        public const string Estimate = "Estimate";
        public const string EstimateId = "EstimateId";
        public const string Design = "Design";
        public const string DesignId = "DesignId";
        public const string Dispatch = "Dispatch";
        public const string DispatchId = "DispatchId";
        public const string MaterialInwardId = "MaterialInwardId";
        public const string LinkedEstimateId = "LinkedEstimateId";
        public const string NetWeight = "NetWeight";
        public const string EmptyBoxWeight = "EmptyBoxWeight";
        public const string Tolerance = "Tolerance";
        public const string Mrp = "MRP";
        public const string WeightPerPiece = "WeightPerPiece";
        public const string GrossWeight = "GrossWeight";
        public const string BundleWeight = "BundleWeight";
        public const string WeightPerSet = "WeightPerSet";
        public const string WeightDifference = "WeightDifference";
        public const string UnitId = "UnitId";
        public const string Unit = "Unit";
        public const string FreshPieces = "Fresh_Pieces";
        public const string FreshNetWeight = "Fresh_NetWeight";
        public const string FreshWeightPerPiece = "Fresh_WeightPerPiece";
        public const string SecondsPieces = "Seconds_Pieces";
        public const string SecondsNetWeight = "Seconds_NetWeight";
        public const string SecondsWeightPerPiece = "Seconds_WeightPerPiece";
        public const string ThirdsPieces = "Thirds_Pieces";
        public const string ThirdsNetWeight = "Thirds_NetWeight";
        public const string ThirdsWeightPerPiece = "Thirds_WeightPerPiece";
        public const string RepolishPieces = "Repolish_Pieces";
        public const string RepolishNetWeight = "Repolish_NetWeight";
        public const string RepolishWeightPerPiece = "Repolish_WeightPerPiece";
        public const string TotalPieces = "Total_Pieces";
        public const string TotalNetWeight = "Total_NetWeight";
        public const string TotalWeightPerPiece = "Total_WeightPerPiece";
        public const string Remark = "Remark";
        public const string DispatchWeight = "DispatchWeight";

        public const string Date = "Date";
        public const string ChallanDate = "ChallanDate";
        public const string EstimateDate = "EstimateDate";
        public const string DispatchDate = "DispatchDate";

        public const string FromDate = "FromDate";
        public const string ToDate = "ToDate";

        public const string ChallanNo = "ChallanNo";
        public const string InvoiceNo = "InvoiceNo";
        public const string SalesInvoice = "SalesInvoice";
        public const string SalesInvoiceId = "SalesInvoiceId";

        public const string MixingSequence = "MixingSequence";
        public const string WeighingSequence = "WeighingSequence";

        public const string Barcode = "Barcode";
        public const string MaterialCode = "MaterialCode";
        public const string FinishLength = "FinishLength";
        public const string FinishWidth = "FinishWidth";
        public const string Thickness = "Thickness";
        public const string ProductionRoute = "ProductionRoute";

        public const string Inspection = "Inspection";
        public const string InspectionId = "InspectionId";

        public const string General = "General";
        public const string Shift1 = "Shift1";
        public const string Shift2 = "Shift2";
        public const string Shift3 = "Shift3";
        public const string Total = "Total";

        public const string StartTime = "StartTime";
        public const string EndTime = "EndTime";

        public const string Relationship = "Relationship";
        public const string ReleaseDate = "ReleaseDate";
        public const string PlanningDate = "PlanningDate";
        public const string MomDate = "MomDate";
        public const string PlanNo = "PlanNo";
        public const string PlanningType = "PlanningType";
        public const string ReviewType = "ReviewType";
        public const string ReviewDate = "ReviewDate";
        public const string KittingDate = "KittingDate";
        public const string KittingRelease = "KittingRelease";
        public const string ManufacturingDueDate = "ManufacturingDueDate";
        public const string ManufacturingDate = "ManufacturingDate";
        public const string PaintingRelease = "PaintingRelease";
        public const string PaintingDate = "PaintingDate";
        public const string Apo = "Apo";
        public const string ReportingDate = "ReportingDate";
        public const string NsCategoryId = "NsCategoryId";
        public const string NsCategory = "NsCategory";
        public const string DrawingNo = "DrawingNo";
        public const string SellType = "SellType";
        public const string ProjectNo = "ProjectNo";
        public const string CustomerDueDate = "CustomerDueDate";
        public const string ProductBreifingDate = "ProductBreifingDate";
        public const string MockupDate = "MockupDate";
        public const string FinalDrawingsReleaseDate = "FinalDrawingsReleaseDate";
        public const string SourceId = "SourceId";
        public const string Source = "Source";
        public const string FreshPowderIn = "FreshPowderIn";
        public const string LoosePowderOut = "LoosePowderOut";
        public const string NetPowderUse = "NetPowderUse";
        public const string OtHr = "OtHr";
        public const string ProductionOrderNo = "ProductionOrderNo";
        public const string NoOfColorChange = "NoOfColorChange";
        public const string PaintShopQuantity = "PaintShopQuantity";
        public const string PaintShop13BQuantity = "PaintShop13BQuantity";
        public const string FinalFittingQuantity = "FinalFittingQuantity";
        public const string FinalFitting13BQuantity = "FinalFitting13BQuantity";
        public const string BendingAndAssemblyQuantity = "BendingAndAssemblyQuantity";
        public const string MetalFormingQuantity = "MetalFormingQuantity";
        public const string PedestalQuantity = "PedestalQuantity";

        // Online Exam
        public const string InstanceId = "InstanceId";
        public const string InstanceName = "InstanceName";
        public const string CollegeId = "CollegeId";
        public const string College = "College";
        public const string CollegeName = "CollegeName";
        public const string PrnNo = "PrnNo";

        // SMS
        public const string None = "None";
        public const string Once = "Once";
        public const string Yearly = "Yearly";
        public const string Montly = "Montly";
        public const string Weekly = "Weekly";
        public const string Daily = "Daily";
        public const string Hourly = "Hourly";
        public const string Minutely = "Minutely";

        // Departments
        public const string Administration = "Administration";

        // Transaction Types
        public const string Attendance = "Attendance";
        public const string Production = "Production";
        public const string Scrap = "Scrap";
        public const string IndexDate = "IndexDate";
        public const string Parameter = "Parameter";
        public const string ParameterId = "ParameterId";
        public const string ParameterType = "ParameterType";
        public const string MaxScore = "MaxScore";
        public const string DepartmentScore = "DepartmentScore";
        public const string IndexId = "IndexId";
        public const string IndexScore = "IndexScore";
        public const string StandardWeightage = "StandardWeightage";
        public const string DepartmentWeightage = "DepartmentWeightage";
        public const string IndexWeightage = "IndexWeightage";

        // Adam Boards
        public const string Adam1 = "Adam1";
        public const string Adam2 = "Adam2";
        public const string Adam3 = "Adam3";
        public const string Adam4 = "Adam4";
        public const string Adam5 = "Adam5";

        // Suyog Rubber
        public const string RecipeId = "RecipeId";
        public const string Fb = "FB";
        public const string Mb = "MB";
        public const string WeighingType = "WeighingType";
        public const string BatchType = "BatchType";

        // SPC
        public const string X1 = "X1";
        public const string X2 = "X2";
        public const string X3 = "X3";
        public const string X4 = "X4";
        public const string X5 = "X5";
        public const string Average = "Average";
        public const string Range = "Range";
        public const string Operator = "Operator";
        public const string Service = "Service";
        public const string RootCause = "RootCause";
        public const string Action = "Action";

        public const string OrderNo = "OrderNo";
        public const string PoNo = "PoNo";
        public const string SoNo = "SoNo";
        public const string SkuCode = "SkuCode";
        public const string CartonNo = "CartonNo";
        public const string CartonSerialNo = "CartonSerialNo";
        public const string PackingDate = "PackingDate";

        // Vikroli.Store
        public const string VendorEntry = "VendorEntry";
        public const string VendorEntryId = "VendorEntryId";
        public const string UnloadingAreaId = "UnloadingAreaId";
        public const string UnloadingArea = "Unloading Area";
        public const string Rejection = "Rejection";
        public const string RejectionId = "RejectionId";
        public const string MainGate1 = "MainGate1";
        public const string MainGate1Id = "MainGate1Id";
        public const string MainGate2 = "MainGate2";
        public const string MainGate2Id = "MainGate2Id";
        public const string Transaction = "Transaction";
        public const string TransactionId = "TransactionId";

        public const string Control = "Control";
        public const string Run = "Run";

        // cWallet
        public const string AddMoney = "Add Money";
        public const string Pay = "Pay";
        public const string Recieve = "Recieve";

        // HUL Tracebility
        public const string ChemicalId = "ChemicalId";
        public const string Chemical = "Chemical";
        public const string WeighingId = "WeighingId";

        // Shirwal.Kitweigh
        public const string Wf = "Wf";
        public const string Baan = "Baan";
        public const string Infor = "Infor";

        public const string SimulationMode = "SimulationMode";
        public const string IpAddress = "IpAddress";
        public const string Port = "Port";

        public const string Department = "Department";
        public const string From = "From";
        public const string To = "To";

        public const string Buffalo = "Buffalo";
        public const string Cow = "Cow";

        public const string DimensionStartAddress = "DimensionStartAddress";
        public const string ColorStartAddress = "ColorStartAddress";
        public const string WeightStartAddress = "WeightStartAddress"; 
        public const string AcceptStartAddress = "AcceptStartAddress";
        public const string IsWeightEnabled = "IsWeightEnabled";
        public const string IsMetalKitchen = "IsMetalKitchen";

        public const string Color = "Color";
        public const string NoOfLabels = "NoOfLabels";
        public const string BoxNo = "BoxNo";
        public const string PalletNo = "PalletNo";
        public const string PalletScanningDate = "PalletScanningDate";
        public const string BarcodeGenerationDate = "BarcodeGenerationDate";
        public const string RackId = "RackId";
        public const string RackScanningDate = "RackScanningDate";
        public const string RetrievalDate = "RetrievalDate";

        public const string Verification = "Verification";
        public const string Payment = "Payment";
        public const string StatusCheck = "StatusCheck";

        public const string Index = "Index";

        public const string Yes = "Yes";
        public const string No = "No";

        public const string Position = "Position";
        public const string PacketNo = "PacketNo";
        public const string MrpLabelPacket = "MrpLabelPacket";
        public const string CurrentProduction = "CurrentProduction";
        public const string ScanQuantity = "ScanQuantity";
        public const string ProductCode = "ProductCode";
        public const string ProductDescription = "ProductDescription";
        public const string SoPosition = "SoPosition";

        public const string WarehouseOrderNo = "WarehouseOrderNo";
        public const string WarehousePosition = "WarehousePosition";
        public const string WarehouseId = "WarehouseId";
        public const string Warehouse = "Warehouse";
        public const string BarcodeDayCounter = "BarcodeDayCounter";
        public const string LabelIndex = "LabelIndex";
        public const string LabelCount = "LabelCount";

        public const string RejectPrinter = "RejectPrinter";
        public const string AcceptPrinter = "AcceptPrinter";

        // 4Ever
        public const string Ever4KeyText = "7c3acb8b876hbd8a95b6a26d15029cf060444ec";
        public const string Iv = "9T3acb8b8uikuyesgteslaodb";
        public const string SessionIv = "8T3acb8b8uikuyesgteslaodM";
        public const string LoginIv = "6P81acb8b8uikuyesgteMasTE";
        public const string KYCIv = "RBDzsdafgWBazQQhQqcfzBXjA";

        //society keys
        public const string SSV = "4faa833637a646d7810315560";
        public const string SSC = "6af85775bef643e561d70d729";
        public const string HUM = "3286288644ca0a9f6abbf63cb";
        public const string LUCC = "ee98fdf491be4486f427ced91";

        //Pan verification
        public const string AccessToken = "b0322068ebd613224ddfa47d1b9266cd";
        public const string TenantCode = "mbware4Ever";
        public const string PanUserName = "TEST4ever";
        public const string PinCode = "400059";
        public const string P_GConst = "p";

        //Account Verification
        public const string Autho = "c68bc0d98920a348f67ef2a59b6f3aee";
        public const string MerchantId = "2";
        public const string Currency = "INR";

        //Email Verification
        public const string LocalEmailUrl = "http://localhost:44300/api/Login/ConfirmEmail?key={0}";
        public const string TestEmailUrl = "http://106.201.237.125/api/Login/ConfirmEmail?key={0}";
        public const string LiveEmailUrl = "https://4everpayapi.co.in/api/Login/ConfirmEmail?key={0}";

        //Card Key
        public const string CardKey = "oPvvNMsiOd//bheeboamB65AXi8r+go9NLz2rTPEJRU=";
        public const string CardIV = "0123456789abcdef";

        //Notification
        public const string Authorization = "AAAAxTta3yI:APA91bGbbJtCO85m0OzGpVEUCcICbv1ozKNDIi_-g8MsHmqBHB0n4GdiUDUbSQ77j-83glc8CFCFXiee4D_9qLPMPYLFUpT7A-6X2dyzWKCZfl9cYmszuSuAjo2u8efksrqEKNVArbisIroAwef7On-I877rtm374g";
        public const string Sender = "847104368418";
        public const string NotificationURL = "https://fcm.googleapis.com/fcm/send";
    }
}
