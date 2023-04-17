using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.FacilityMaterial, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, Facility.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, "MinStockQuantity", ConstApp.MinStockQuantity, "", "", true)]
    [ACPropertyEntity(4, "OptStockQuantity", ConstApp.OptStockQuantity, "", "", true)]
    [ACPropertyEntity(5, "MaxStockQuantity", ConstApp.MaxStockQuantity, "", "", true)]
    [ACPropertyEntity(6, "Throughput", "en{'Throughput [UOM/h]'}de{'Durchsatz [BME/h]'}", "", "", true)]
    [ACPropertyEntity(7, "ThroughputMax", "en{'Throughput max. [UOM/h]'}de{'Durchsatz max. [BME/h]'}", "", "", true)]
    [ACPropertyEntity(8, "ThroughputMin", "en{'Throughput min. [UOM/h]'}de{'Durchsatz min. [BME/h]'}", "", "", true)]
    [ACPropertyEntity(9, "ThroughputAuto", "en{'Throughput Autoupdate mode'}de{'Durchsatz Autoupdate Modus'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    public partial class FacilityMaterial
    {
        #region New/Delete

        public static FacilityMaterial NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            FacilityMaterial entity = new FacilityMaterial();
            entity.FacilityMaterialID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Facility)
            {
                entity.Facility = parentACObject as Facility;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion
    }
}
