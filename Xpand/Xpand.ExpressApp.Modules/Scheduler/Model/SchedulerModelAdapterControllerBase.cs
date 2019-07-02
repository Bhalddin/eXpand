﻿using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Model {
    public abstract class SchedulerModelAdapterControllerBase : ModelAdapterController, IModelExtender {

        public SchedulerListEditorBase SchedulerListEditor {
            get {
                var listView = View as ListView;
                return listView?.Editor as SchedulerListEditorBase;
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (SchedulerListEditor != null) {
                ((ListView)View).CollectionSource.CriteriaApplied += CollectionSourceOnCriteriaApplied;
                throw new NotImplementedException();
//                new SchedulerListEditorModelSynchronizer(SchedulerControl(), (IModelListViewOptionsScheduler)View.Model, Labels(), Statuses()).ApplyModel();
                SchedulerListEditor.ResourceDataSourceCreating += SchedulerListEditorOnResourceDataSourceCreating;
            }
            var detailView = View as DetailView;
            if (detailView != null && View.ObjectTypeInfo.Implements<IEvent>()) {
                Frame.GetController<LinkToListViewController>(controller => controller.LinkChanged += LinkToListViewControllerOnLinkChanged);
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (SchedulerListEditor != null) {
                ((ListView)View).CollectionSource.CriteriaApplied -= CollectionSourceOnCriteriaApplied;
                SchedulerListEditor.ResourceDataSourceCreating -= SchedulerListEditorOnResourceDataSourceCreating;
            }
            Frame.GetController<LinkToListViewController>(controller => controller.LinkChanged -= LinkToListViewControllerOnLinkChanged);

        }

        void LinkToListViewControllerOnLinkChanged(object sender, EventArgs eventArgs) {
            var link = ((LinkToListViewController) sender).Link;
            if (SchedulerListEditor!=null && link?.ListView != null) {
                throw new NotImplementedException();
//                new AppoitmentSynchronizer(Labels(), Statuses(), (IModelListViewOptionsScheduler)link.ListView.Model).ApplyModel();
            }	
        }

        protected abstract IAppointmentStatusStorage Statuses();

        protected abstract IAppointmentLabelStorage Labels();

        protected abstract IInnerSchedulerControlOwner SchedulerControl();
        [Obsolete]
        void SchedulerListEditorOnResourceDataSourceCreating(object sender, ResourceDataSourceCreatingEventArgs e) {
//            var resourceListView = ((IModelListViewOptionsScheduler)View.Model).ResourceListView;
//            if (resourceListView != null) {
//                var collectionSourceBase = Application.CreateCollectionSource(Application.CreateObjectSpace(e.ResourceType), e.ResourceType, resourceListView.Id, false, CollectionSourceMode.Proxy);
//                Application.CreateListView(resourceListView.Id, collectionSourceBase, true);
//                e.DataSource = collectionSourceBase.Collection;
//                e.Handled = true;
//            }
        }
        [Obsolete]
        void CollectionSourceOnCriteriaApplied(object sender, EventArgs eventArgs) {
//            if (((ListView)View).CollectionSource.Criteria.ContainsKey("ActiveViewFilter")) {
//                var modelListViewOptionsScheduler = ((IModelListViewOptionsScheduler)View.Model);
//                if (modelListViewOptionsScheduler.ResourcesOnlyWithAppoitments) {
//                    var storage = Storage();
//                    storage.BeginUpdate();
//                    var resources = Resources();
//                    for (int i = 0; i < resources.Count; i++) {
//                        resources[i].Visible = false;
//                    }
//                    foreach (var resourceId in Items().SelectMany(appointment => appointment.ResourceIds)) {
//                        resources.GetResourceById(resourceId).Visible = true;
//                    }
//                    storage.EndUpdate();
//                }
//            }
        }

        protected abstract ResourceStorageBase Resources();

        protected abstract IEnumerable<Appointment> Items();
        protected abstract ISchedulerStorageBase Storage();
        [Obsolete]
        protected void SynchMenu(object menu) {
//            var popupMenus = ((IModelListViewOptionsScheduler) View.Model).OptionsScheduler.PopupMenuItems;
//            foreach (var popupMenu in popupMenus){
//                var component = GetMenu(menu,popupMenu);
//                if (component != null) new SchedulerPopupMenuModelSynchronizer(component, popupMenu).ApplyModel();
//            }
        }

        protected virtual object GetMenu(object popupMenu, IModelSchedulerPopupMenuItem modelMenu){
            return popupMenu;
        }
        [Obsolete]
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
//            extenders.Add<IModelListView, IModelListViewOptionsScheduler>();

            var builder = new InterfaceBuilder(extenders);
            var interfaceBuilderDatas = CreateBuilderData();
            
            Build(builder, interfaceBuilderDatas);
        }

        protected abstract void Build(InterfaceBuilder builder, IEnumerable<InterfaceBuilderData> interfaceBuilderDatas);


        protected virtual IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(SchedulerControlType()) {
                Act = info =>{
                    if (info.Name == "Item"&&!info.PropertyType.BehaveLikeValueType())
                        return false;
                    if (info.Name == "DataStorage") {
                        info.SetName("Storage");
                        info.SetPropertyType(typeof(SchedulerStorage));
                    }
                    info.RemoveInvalidTypeConverterAttributes("DevExpress.XtraScheduler.Design");
                    return info.DXFilter(BaseSchedulerControlTypes(), typeof(object));
                }
            };
        }

        protected abstract Type SchedulerControlType();

        protected virtual Type[] BaseSchedulerControlTypes() {
            return new[]{
                typeof (ResourceStorageBase), typeof (AppointmentMappingInfo), typeof (AppointmentStorageBase),
                typeof (BaseOptions), typeof (SchedulerStorageBase),typeof (ResourceMappingInfo)
            };
        }
    }

}
