delete msg
from ACClassMessage msg
inner join ACClass cl on cl.ACClassID = msg.ACClassID
where cl.ACIdentifier ='DischargingItemNoValidator'