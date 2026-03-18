delete from ACClass where ACIdentifier = 'EInvoiceTypeValueList'
delete from ACClass where ACIdentifier = 'EInvoiceBussinesProcessTypeValueList'

delete from cl
from ACClass cl
inner join ACClass paCl on pacl.ACClassID = cl.ParentACClassID
where paCl.ACIdentifier = 'EInvoiceManager' and paCl.AssemblyQualifiedName = ''

delete from ACClass where ACIdentifier = 'EInvoiceManager' and AssemblyQualifiedName = ''
delete from ACClass where ACIdentifier = 'EInvoiceManager'