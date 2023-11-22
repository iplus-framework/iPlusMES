declare @updateJob TABLE
(

    MethodName varchar(150),
    ACClassWFEdgeID UNIQUEIDENTIFIER,
    EdgeACIdentifier varchar(150),
    PWACClassID UNIQUEIDENTIFIER,
    PWACClassID_ACIdentifier varchar(150),
    TargetPropertyACClassACIdentifier varchar(150),
    TargetPropertyACIdentifier varchar(150),
    TargetACClassPropertyID UNIQUEIDENTIFIER,

    NewTargetACClassPropertyID UNIQUEIDENTIFIER,
    NewTargetPropertyACIdentifier varchar(150),
    NewTargetPropertyACClassACIdentifier varchar(150)
);


insert into @updateJob 
(
    MethodName, 
    ACClassWFEdgeID,
    EdgeACIdentifier,
    PWACClassID,
    PWACClassID_ACIdentifier,
    TargetPropertyACClassACIdentifier,
    TargetPropertyACIdentifier,
    TargetACClassPropertyID,
    NewTargetACClassPropertyID,
    NewTargetPropertyACIdentifier,
    NewTargetPropertyACClassACIdentifier
)
select 
    DISTINCT
   mth.ACIdentifier as MethodName,
    e.ACClassWFEdgeID,
    e.ACIdentifier EdgeACIdentifier,
    tc.ACClassID PWACClassID, 
    tc.ACIdentifier PWACClassID_ACIdentifier, 
    tpAcls.ACIdentifier TargetPropertyACClassACIdentifier, 
    tp.ACIdentifier TargetPropertyACIdentifier, 
    e.TargetACClassPropertyID,  -- update
    tmp.ACClassPropertyID NewTargetACClassPropertyID,
    tmp.ACIdentifier NewTargetPropertyACIdentifier,
    tmp.clsId as NewTargetPropertyACClassACIdentifier
    --t.* , e.*
from ACClassWFEdge e 
inner join ACClassWF t on t.ACClassWFID = e.TargetACClassWFID
inner join ACClassProperty tp on tp.ACClassPropertyID = e.TargetACClassPropertyID
inner join ACClass tpAcls on tpAcls.ACClassID = tp.ACClassID
inner join ACClass tc on tc.ACClassID = t.PWACClassID
inner join ACClassMethod mth on mth.ACClassMethodID = e.ACClassMethodID
inner join 
(
        select 
        distinct
        p.ACClassID, 
        p.ACClassPropertyID, -- result
        p.ACIdentifier,
        c1.ACIdentifier as clsId,
        c1.ACClassID  as id1,
        c2.ACClassID  as id2,
        c3.ACClassID  as id3,
        c4.ACClassID  as id4,
        c5.ACClassID  as id5,
        c6.ACClassID  as id6,
        c7.ACClassID  as id7
    from ACClassProperty p
    left join ACClass c1 on c1.ACClassID = p.ACClassID
    left join ACClass c2 on c2.BasedOnACClassID = c1.ACClassID
    left join ACClass c3 on c3.BasedOnACClassID = c2.ACClassID
    left join ACClass c4 on c4.BasedOnACClassID = c3.ACClassID
    left join ACClass c5 on c5.BasedOnACClassID = c4.ACClassID
    left join ACClass c6 on c6.BasedOnACClassID = c5.ACClassID
    left join ACClass c7 on c7.BasedOnACClassID = c6.ACClassID
    where p.ACIdentifier = 'PWPointIn' and c1.ACIdentifier <> 'PWGroup'
    -- and (c1.ACClassID = '19CBD6EC-530F-4797-81C1-5384E5641127' -- tc.ACClassID
    -- or c2.ACClassID = '19CBD6EC-530F-4797-81C1-5384E5641127' 
    -- or c3.ACClassID = '19CBD6EC-530F-4797-81C1-5384E5641127' 
    -- or c4.ACClassID = '19CBD6EC-530F-4797-81C1-5384E5641127'
    -- or c5.ACClassID = '19CBD6EC-530F-4797-81C1-5384E5641127'
    -- or c6.ACClassID = '19CBD6EC-530F-4797-81C1-5384E5641127'
    -- or c7.ACClassID = '19CBD6EC-530F-4797-81C1-5384E5641127'
    ) tmp on 
                
                    tmp.id1 = tc.ACClassID
                    or tmp.id2 = tc.ACClassID
                    or tmp.id3 = tc.ACClassID
                    or tmp.id4 = tc.ACClassID
                    or tmp.id5 = tc.ACClassID
                    or tmp.id6 = tc.ACClassID
                    or tmp.id7 = tc.ACClassID
                
                

where
		(tpAcls.ACIdentifier = 'PWNodeEnd' 
		and tc.ACIdentifier <> 'PWNodeEnd' 
		and tp.ACIdentifier = 'PWPointIn')
	or (tp.ACIdentifier = 'PWPointIn'
		and tpAcls.ACIdentifier like 'PWGroup'
		and tc.ACIdentifier not like '%Group%')
order by e.ACClassWFEdgeID;

update edg
    set edg.TargetACClassPropertyID = tmp.NewTargetACClassPropertyID
from ACClassWFEdge edg
inner join @updateJob tmp on tmp.ACClassWFEdgeID = edg.ACClassWFEdgeID;

update ACClassTaskValue set ACClassPropertyID = '598ad8c8-1391-4b8c-9ae4-9a8ba57f7372' where ACClassPropertyID  = '61b790c9-a379-483d-b3af-1594b567a730';
update ACClassWFEdge set TargetACClassPropertyID = '598ad8c8-1391-4b8c-9ae4-9a8ba57f7372' where TargetACClassPropertyID  = '61b790c9-a379-483d-b3af-1594b567a730';
delete ACClassProperty where ACClassPropertyID  = '61b790c9-a379-483d-b3af-1594b567a730';

