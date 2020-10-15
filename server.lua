data = {}
TriggerEvent("redemrp_inventory:getData",function(call)
    data = call
end)


RegisterServerEvent("redemrp_fishing:AddFish")
AddEventHandler("redemrp_fishing:AddFish", function(name , weight)
local _source = source
local meta = {}
meta.fishweight = round(weight, 2)
local ItemData = data.getItem(_source, name, meta)
ItemData.AddItem(1)
end)

function round(num, numDecimalPlaces)
  local mult = 10^(numDecimalPlaces or 0)
  return math.floor(num * mult + 0.5) / mult
end

RegisterServerEvent("redemrp_fishing:Sell")
AddEventHandler("redemrp_fishing:Sell", function()
	local _source = source
	local ItemData = data.getItem(_source, "smallfish")
	local ItemData2 = data.getItem(_source, "mediumfish")
	local ItemData3 = data.getItem(_source, "largefish")
		TriggerEvent('redemrp:getPlayerFromId', _source, function(user)	
				if ItemData.ItemAmount > 0 then
				print(json.encode(ItemData.ItemMeta))
					local final = ItemData.ItemMeta.fishweight * ItemData.ItemAmount
					if ItemData.ItemMeta.fishweight < 0.6 then
						user.addMoney(final*3.5)
						user.addXP(final * 3.5)
						ItemData.RemoveItem(ItemData.ItemAmount)
					elseif ItemData.ItemMeta.fishweight < 3.0  then
						user.addMoney(final*2.5)
						user.addXP(final * 2.5)
						ItemData.RemoveItem(ItemData.ItemAmount)
					end
				end
				
				if ItemData2.ItemAmount > 0 then
					local final = ItemData2.ItemMeta.fishweight * ItemData2.ItemAmount
						user.addMoney(final*2)
						user.addXP(final * 2)
						ItemData2.RemoveItem(ItemData2.ItemAmount)
				end
				
				if ItemData3.ItemAmount > 0 then
					local final = ItemData3.ItemMeta.fishweight * ItemData3.ItemAmount
						user.addMoney(final*1.5)
						user.addXP(final * 1.5)
						ItemData3.RemoveItem(ItemData3.ItemAmount)
				end
		end)
end)





RegisterServerEvent("redemrp_fishing:RemoveBait")
AddEventHandler("redemrp_fishing:RemoveBait", function(name)
    local _source = source
    local ItemData = data.getItem(_source, name)
	ItemData.RemoveItem(1)
	
end)

RegisterServerEvent("RegisterUsableItem:p_baitBread01x")
AddEventHandler("RegisterUsableItem:p_baitBread01x", function(source)
    local _source = source
    TriggerClientEvent("redemrp_fishing:UseBait", _source, "p_baitBread01x")
	
end)

RegisterServerEvent("RegisterUsableItem:p_baitCorn01x")
AddEventHandler("RegisterUsableItem:p_baitCorn01x", function(source)
    local _source = source
    TriggerClientEvent("redemrp_fishing:UseBait", _source, "p_baitCorn01x")
end)

RegisterServerEvent("RegisterUsableItem:p_baitCheese01x")
AddEventHandler("RegisterUsableItem:p_baitCheese01x", function(source)
    local _source = source
    TriggerClientEvent("redemrp_fishing:UseBait", _source, "p_baitCheese01x")
end)


RegisterServerEvent("RegisterUsableItem:p_finishedragonfly01x")
AddEventHandler("RegisterUsableItem:p_finishedragonfly01x", function(source)
    local _source = source
    TriggerClientEvent("redemrp_fishing:UseBait", _source, "p_finishedragonfly01x")
end)


RegisterServerEvent("RegisterUsableItem:p_baitCricket01x")
AddEventHandler("RegisterUsableItem:p_baitCricket01x", function(source)
    local _source = source
    TriggerClientEvent("redemrp_fishing:UseBait", _source, "p_baitCricket01x")
end)

RegisterServerEvent("RegisterUsableItem:p_FinisdFishlure01x")
AddEventHandler("RegisterUsableItem:p_FinisdFishlure01x", function(source)
    local _source = source
    TriggerClientEvent("redemrp_fishing:UseBait", _source, "p_FinisdFishlure01x")
end)

RegisterServerEvent("RegisterUsableItem:p_finishdcrawd01x")
AddEventHandler("RegisterUsableItem:p_finishdcrawd01x", function(source)
    local _source = source
    TriggerClientEvent("redemrp_fishing:UseBait", _source, "p_finishdcrawd01x")
end)
