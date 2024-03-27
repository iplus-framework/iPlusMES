delete ACClass where ACIdentifier = 'LabOrderMESViewDialog';
delete ACClass where ACIdentifier = 'LabOrderViewDialog';
update ACClass set ACIdentifier = 'LabOrderDialog' where ACIdentifier = 'LabOrderDialogProd';