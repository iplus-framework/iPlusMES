delete method from ACClassMethod method inner join ACClassMethod m on method.ParentACClassMethodID = m.ACClassMethodID inner join ACClass c on m.ACClassID = c.ACClassID where c.ACIdentifier = 'PAFBakeryWorkDecoring' and m.ACIdentifier = 'Packing'
GO

delete m from ACClassMethod m inner join ACClass c on m.ACClassID = c.ACClassID where c.ACIdentifier = 'PAFBakeryWorkDecoring' and m.ACIdentifier = 'Packing'
GO

update m set PWACClassID = (select ACClassID from ACClass where ACIdentifier = 'PWBakeryWorkDecoring') from ACClassMethod m inner join ACClass c on m.PWACClassID = c.ACClassID where m.ACIdentifier = 'Decoring' and c.ACIdentifier = 'PWBakeryWorkPacking'

