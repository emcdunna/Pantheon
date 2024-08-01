import sys
import os
import copy
import random


#C:\Users\alish\AppData\Local\Programs\Python\Python310\python.exe

settlement_scale = [10,20,30,40]

MAX_CONNECT_DISTANCE_LAND = 200000

PROVINCE_TERRAIN = dict()
ALL_SETTLEMENTS = dict()
NOMAD_REGIONS = dict()
SETTLED_REGIONS = dict()
ALL_BUILDINGS = dict()
ALL_RESOURCES = dict()
ALL_POPULATIONS = dict()
ALL_CULTURES = dict()
OCCUPATION_TO_POP = dict()
ALL_CULTURE_TRAITS = dict()
PROVINCE_TO_REGIONS = dict()


class Region:
    def __init__(self, region_name, xpos, ypos):
        self.region_name = region_name
        self.xpos = int(xpos)
        self.ypos = int(ypos)
        self.terrain = None
        self.buildings = dict() # building -> number
        self.land_remaining = dict()
        self.adjacentSettlements = []
        self.valid_resources = []
        self.adjacentBySeaRegions = []
        self.type = "nomadic"
        self.culture = None
        self.ports = 0
        self.size = 0
        self.population = dict() # pop group -> number
        self.minerals = dict()   # resource -> number

    def __str__(self):
        return self.region_name

    def connectToSettlement(self, settlement, type="Land"):
        if self != settlement:
            if type == "Sea":
                ours = self.adjacentBySeaRegions
                theirs = settlement.adjacentBySeaRegions
            else:
                ours = self.adjacentSettlements
                theirs = settlement.adjacentSettlements
            if settlement not in ours:
                ours.append(settlement)
            if self not in theirs:
                theirs.append(self)

    def countMineralUse(self, mineral):
        used = 0
        for b in self.buildings.keys():
            num = self.buildings[b]
            building = ALL_BUILDINGS[b]
            if building.resource_req == mineral:
                used += 1
        return used

    def isBuildingAllowed(self, building):
        supply = self.getSupplyDemand()
        food = 0
        try:
            food = supply["Food"]
        except:
            pass

        # Assume we already know the resource req is satisified
        # find out how much land is left of this type
        if building.resource_req is not None and building.resource_req != "":
            if building.resource_req in self.minerals:
                count = self.countMineralUse(building.resource_req)
                #print(str(building.resource_req) + " using " + str(count) + " / " + str(self.minerals[building.resource_req]))
                if count < self.minerals[building.resource_req]:
                    pass
                else:
                    return False
            elif building.resource_req not in self.valid_resources:
                #print(str(building) + " not allowed due to resource req " + str(building.resource_req))
                return False
        # Find out if building is allowed by culture
        if building.culture_req is not None and building.culture_req not in self.culture.traits:
            #print(str(building) + " not allowed due to culture req " + str(building.culture_req))
            return False
        # Find out if there is space left in the land
        if building.land_req == "Town" and self.type == "nomadic" and food > 0:
            return True
        elif self.land_remaining[building.land_req] >= 1:
            return True
        else:
            return False

    def setLandRemaining(self):
        self.land_remaining["Forest"] = self.terrain.woods
        self.land_remaining["Desert"] = self.terrain.deserts
        self.land_remaining["Mountain"] = self.terrain.mountains
        self.land_remaining["Wetland"] = self.terrain.wetlands
        self.land_remaining["Port"] = self.ports
        self.land_remaining["Town"] = self.size
        self.land_remaining["Field"] = self.terrain.fields + self.terrain.hills

    # returns {"Fields":1, "Desert":3...}
    def getLandRemaining(self):
        #self.land_remaining["Field"] = self.land_remaining["Plain"] + self.land_remaining["Hills"]
        return self.land_remaining

    def addBuilding(self, building):
        if building.culture_req is not None and building.culture_req not in self.culture.implicit_traits:
            self.culture.implicit_traits.append(building.culture_req)

        self.land_remaining[building.land_req] -= 1
        try:
            self.buildings[building.name] += 1
        except KeyError:
            self.buildings[building.name] = 1

        pop = OCCUPATION_TO_POP[building.type]
        num = int(building.pops)
        try:
            self.population[pop] += num
        except KeyError:
            self.population[pop] = num

    def getSettlementTerrain(self):
        return self.region_name + "\t" + self.name \
                + "\t" + str(self.terrain.temp) + "\t" + str(self.terrain.prec) \
                + "\t" + str(self.terrain.fields) + "\t" + str(self.terrain.woods) + "\t" + \
                str(self.terrain.hills) + "\t" + str(self.terrain.mountains) + "\t" + \
                str(self.terrain.deserts) + "\t" + str(self.terrain.wetlands) + "\n"

    def getBuildings(self):
        word = ""
        for k in self.buildings.keys():
            word += self.region_name + "\t" + str(k) + "\t" + str(self.buildings[k]) + "\n"
        return word

    def howManyBuildings(self, b_name):
        if b_name in self.buildings.keys():
            return self.buildings[b_name]
        else:
            return 0

    def getSupplyDemand(self):
        regionsupply = dict() # positive means excess, negative means deficit
        regionsupply["Food"] = 0
        regionsupply["TradeGoods"] = 0
        regionsupply["LuxuryGoods"] = 0
        for pop in self.population.keys():
            num = self.population[pop]
            regionsupply["Food"] -= (pop.food_demand * num)
            regionsupply["TradeGoods"] -= (pop.trade_good_demand * num)
            regionsupply["LuxuryGoods"] -= (pop.luxury_good_demand * num)

        for bname in self.buildings.keys():
            building = ALL_BUILDINGS[bname]
            num = self.buildings[bname]
            for resource in building.output.keys():
                val = (building.output[resource] * num)
                try:
                    regionsupply[resource] += val
                except KeyError:
                    regionsupply[resource] = val
        return regionsupply

    def getPopulation(self):
        word = ""
        for pop in self.population.keys():
            num = self.population[pop]
            word += str(num) + " " + pop.name + ", "
        return word

class Culture:
    def __init__(self, name, type, group):
        self.name = name
        self.type = type
        self.traits = []
        self.implicit_traits = []
        self.settlements = []
        self.group = group
        self.unity = 0
        self.advancement = 0
        self.isolationism = 0
        self.independence = 0

    def getCultureDescription(self):
        word = "" + self.name + ": \n"
        word += str(len(self.settlements)) + " " + str(self.type) + " regions.\n"
        word += "Traits: "
        x = len(self.traits)
        i = 0
        for t in self.traits:
            i += 1
            word += str(t)
            if i == x:
                word += "."
            else:
                word += ", "
        word += "\n\t* Unity: " + str(self.unity)
        word += "\n\t* Advancement: " + str(self.advancement)
        word += "\n\t* Isolationism: " + str(self.isolationism)
        word += "\n\t* Independence: " + str(self.independence)
        populations = dict()
        for region in self.settlements:
            for pop in region.population.keys():
                num = region.population[pop]
                try:
                    populations[pop] += num
                except:
                    populations[pop] = num

        for pop in populations.keys():
            val = populations[pop]
            word += "\n" + str(val) + " " + str(pop)

        av_terrain = getAverageTerrain(self.settlements)
        word += "\nGenerally " + av_terrain.terrainDescription()
        word += "\n" + str(av_terrain.getTerrain())
        word += "\nMarket:\n"
        word += getNetMarket(self.settlements)

        word += "\nBuildings:\n"
        word += getBuildingTotals(self.settlements)
        return word

    def addTrait(self, trait):
        self.traits.append(trait)
        self.unity += trait.unity
        self.advancement += trait.advancement
        self.isolationism += trait.isolationism
        self.independence += trait.independence

    def __str__(self):
        return self.name

class Trait:
    def __init__(self, Name, Type, Unity, Advancement, Isolationism, Independence):
        self.name = Name
        self.type = Type
        self.unity = int(Unity)
        self.advancement = int(Advancement)
        self.isolationism = int(Isolationism)
        self.independence = int(Independence)

    def __str__(self):
        return self.name

class Population:
    def __init__(self, name, occupation, prestige, food_demand, trade_good_demand, luxury_good_demand):
        #Population	Occupation	Prestige	Food Demand
        #Trade Goods Demand	Luxury Goods Demand	Description
        self.name = name
        self.occupation = occupation
        self.prestige = int(prestige)
        self.food_demand = int(food_demand)
        self.trade_good_demand = int(trade_good_demand)
        self.luxury_good_demand = int(luxury_good_demand)

    def __str__(self):
        return self.name

class Building:
    def __init__(self, name, land, type, culture_requirement, pops, resource_req):
        self.name = name
        self.land_req = land
        self.type = type
        self.culture_req = culture_requirement
        self.pops = int(pops)
        if resource_req is None or resource_req == "":
            self.resource_req = None
        else:
            self.resource_req = ALL_RESOURCES[resource_req]
        self.output = dict()

    def __str__(self):
        return self.name

    # get the output of a specific resource
    def getOutput(self, type):
        if type not in self.output.keys():
            return 0
        else:
            return self.output[type]

    # Calculates the utility of adding this building to the existing region
    def calculateUtility(self, region, debug = True):
        if debug:
            print("\tCalculating utility of " + self.name + " for " + region.region_name)
        regionsupply = region.getSupplyDemand()
        utility = 0

        # Calculate how this buildings outputs affect the settlement
        for resource in self.output.keys():
            out = self.getOutput(resource)
            try:
                rs = regionsupply[resource]
            except KeyError:
                rs = 0
            # always demand an excess of food
            if resource == "Food":
                rs -= 10

            net = out + rs

            # determine if the building needs this resource or provides it
            producing = False
            if out >= 0:
                producing = True
            deficit = False
            if rs < 0:
                deficit = True

            positive_force = 0
            negative_force = 0

            # not relevant if the building doesnt affect the resource
            if out != 0:
                if debug:
                    print("\t* " + str(resource) + "\n\t\tDeficit: " + str(deficit) + "\n\t\tProducing: " + str(producing))
                    print("\t\tOutput: " + str(out))
                    print("\t\tSupply: " + str(rs))
                    print("\t\tNet:    " + str(net))

                # Resource is in a deficit (demand force)
                if deficit:
                    # solving the problem
                    if producing:
                        # still in a deficit afterwards
                        if net <= 0:
                            positive_force = (3 * out)
                        # solves the deficit
                        else:
                            positive_force = 2 * out
                    # makes the problem worse
                    else:
                        negative_force = net

                # Resource is in excess
                else:
                    if producing:
                         positive_force = out
                    else:
                        # still in excess afterwards
                        if net >= 0:
                            negative_force = out / 2
                        # makes the resource go into a deficit
                        else:
                            negative_force = 2 * net + out

                if debug:
                    print("\tPositive force: " + str(positive_force))
                    print("\tNegative force: " + str(negative_force))
                assert negative_force <= 0
                assert positive_force >= 0

                utility = utility + positive_force + negative_force
                # end if out > 0
            # end for resource in keys()
        # do only once per building

        # Lower the demand for each other building of the same type
        if utility > 0:
            utility *= 5.0 / (5.0 + region.howManyBuildings(self.name))

        return utility

class Resource:
    def __init__(self, name, min_prec, max_prec, min_temp, max_temp, priority, purpose):
        self.name = name
        self.min_prec = int(min_prec)
        self.max_prec = int(max_prec)
        self.min_temp = int(min_temp)
        self.max_temp = int(max_temp)
        self.priority = int(priority)
        self.purpose = purpose

    def __str__(self):
        return self.name

class Settlement(Region):
    def __init__(self, region_name, name, size, type, ranking, xpos, ypos):
        self.name = name
        super(Settlement, self).__init__(region_name, xpos, ypos)
        self.region_name = region_name
        self.type = type
        self.ranking = ranking
        self.province = "Unknown"
        self.size = int(size)
        self.walls = 0
        self.ports = 0

    def getSettlementsTSV(self):
        return self.region_name + "\t" + self.name + "\t" + str(self.size) + \
                    "\t" + self.type + "\t" + self.ranking + "\t" + str(self.xpos) + \
                    "\t" + str(self.ypos) + "\n"


    def addDefaultWalls(self):
        r = self.getRankScore()
        self.walls = r


    def getRankScore(self):
        if self.ranking == "Town":
            return 1
        elif self.ranking == "Large Town":
            return 2
        elif self.ranking == "City":
            return 3
        elif self.ranking == "Major City":
            return 4
        elif self.ranking == "Huge City":
            return 5
        else:
            return 0

    def generateSize(self):
        if self.ranking == "Huge City":
            self.size = random.randint(settlement_scale[3], settlement_scale[3]+5)

        elif self.ranking == "Major City":
            self.size = random.randint(settlement_scale[2], settlement_scale[3]-1)

        elif self.ranking == "City":
            self.size = random.randint(settlement_scale[1], settlement_scale[2]-1)

        elif self.ranking == "Large Town":
            self.size = random.randint(settlement_scale[0], settlement_scale[1]-1)

        else:
            self.size = random.randint(5, settlement_scale[0]-1)

class Terrain:
    def __init__(self, province, temp, precipitation, fields, woods, hills,
                 mountains, deserts, wetlands):
        self.province = province
        self.temp = round(float(temp),2)
        self.prec = round(float(precipitation),2)
        self.fields = round(float(fields),2)
        self.woods = round(float(woods),2)
        self.hills = round(float(hills),2)
        self.mountains = round(float(mountains),2)
        self.deserts =round( float(deserts),2)
        self.wetlands = round(float(wetlands),2)

    def __str__(self):
        return self.province

    def getTerrain(self):
        word = ""
        word += "Temp: " + str(self.temp)
        word += ", Prec: " + str(self.prec)
        word += ", Fields: " + str(self.fields)
        word += ", Woods: " + str(self.prec)
        word += ", Hills: " + str(self.hills)
        word += ", Mountains: " + str(self.mountains)
        word += ", Deserts: " + str(self.deserts)
        word += ", Wetlands: " + str(self.wetlands)
        return word

    def terrainDescription(self):
        word = ""
        if self.temp >= 8:
            word += "Hot "
        elif self.temp >= 6:
            word += "Warm "
        elif self.temp >= 4:
            word += "Temperate "
        elif self.temp >= 2:
            word += "Cold "
        else:
            word += "Arctic "
        if self.prec <= 3:
            word += "dry "
        elif self.prec <= 6:
            pass
        else:
            word += "wet "
        land = "flatland"
        if self.woods >= 6:
            land = "forest"
        if self.wetlands >= 5:
            land = "swamp"
        if self.mountains + self.hills >= 8:
            land = "highlands"
        if self.deserts >= 6:
            land = "desert"

        word += land
        return word


    def isResourceValid(self, resource):
        if resource.purpose == "Mining":
            return False
        if (self.temp >= resource.min_temp and self.temp <= resource.max_temp):
            if (self.prec >= resource.min_prec and self.prec <= resource.max_prec):
                return True
        return False

#####################################################################################
# Functions
#####################################################################################
def loadTsvFile(filepath):
    data = []
    f = open(filepath,"r")
    i = 0
    for line in f:
        if i != 0:
            line = line.replace("\n","")
            d = line.split("\t")
            data.append(d)
        i += 1
    return data

def loadDefaultProvinceTerrain():
    provinces = open("World/ProvinceTerrain.tsv","r")
    i = 0
    for line in provinces:
        if i != 0:
            line = line.replace("\n","")
            data = line.split("\t")
            Name = data[0]
            Temperature = data[1]
            Precipitation = data[2]
            Fields = data[3]
            Woods = data[4]
            Hills = data[5]
            Mountains = data[6]
            Deserts = data[7]
            Wetlands = data[8]
            t = Terrain(Name, Temperature, Precipitation, Fields, Woods, Hills,
                        Mountains, Deserts, Wetlands)
            PROVINCE_TERRAIN[Name] = t
        i += 1
    for k in ALL_SETTLEMENTS.keys():
        s = ALL_SETTLEMENTS[k]
        for prov_name in PROVINCE_TERRAIN.keys():
            shrt_name = prov_name.replace(" ","")
            if shrt_name in s.region_name:
                terrain = PROVINCE_TERRAIN[prov_name]
                s.province = prov_name
                #s.terrain = copy.deepcopy(terrain)
                try:
                    PROVINCE_TO_REGIONS[prov_name].append(s)
                except KeyError:
                    PROVINCE_TO_REGIONS[prov_name] = [s]
    print("Read province terrain defaults.")
    provinces.close()

def writeSettlements2():
    settlements2 = open("World/Settlements2.tsv", "w")
    i = 0
    for key in ALL_SETTLEMENTS:
        s = ALL_SETTLEMENTS[key]
        print(s)
        settlements2.write(str(s))
        i += 1
    settlements2.close()
    print("Wrote " + str(i) + " Settlement2s")

def writeBuildings():
    settlement_buildings = open("World/SettlementBuildings.tsv","w")
    settlement_buildings.write("Settlement\tBuilding\tNumber\n")
    i = 0
    for key in ALL_SETTLEMENTS:
        s = ALL_SETTLEMENTS[key]
        data = s.getBuildings()
        settlement_buildings.write(data)
        i += data.count("\n")
    settlement_buildings.close()
    print("Wrote " + str(i) + " Settlement buildings")

def writeSettlementTerrain():
    settlement_terrain = open("World/SettlementTerrain.tsv","w")
    i = 0
    settlement_terrain.write("Region\tSettlement\tTemperature\tPrecipitation\tFields\tWoods	Hills\tMountains\tDeserts\tWetlands\n")
    for key in ALL_SETTLEMENTS:
        s = ALL_SETTLEMENTS[key]
        data = s.getSettlementTerrain()
        i += 1
        settlement_terrain.write(data)
    settlement_terrain.close()
    print("Wrote " + str(i) + " Settlement Terrains")

def loadSettlementTerrain():
    settlement_terrain = open("World/SettlementTerrain.tsv","r")
    i = 0
    for line in settlement_terrain:
        if i != 0:
            data = line.replace("\n","").split("\t")

            Region = data[0]
            Temperature = data[1]
            Precipitation = data[2]
            Fields = data[3]
            Woods = data[4]
            Hills = data[5]
            Mountains = data[6]
            Deserts = data[7]
            Wetlands = data[8]
            t = Terrain(Region, Temperature, Precipitation, Fields, Woods, Hills,
                        Mountains, Deserts, Wetlands)
            ALL_SETTLEMENTS[Region].terrain = t
            ALL_SETTLEMENTS[Region].setLandRemaining()
        i += 1
    settlement_terrain.close()
    print("Read Settlement terrain lines.")

def loadBuildings():
    buildings = loadTsvFile("World/Buildings.tsv")
    outputs = loadTsvFile("World/BuildingOutput.tsv")
    i = 0
    for data in buildings:
        Name = data[0]
        land_req = data[1]
        type = data[2]
        try:
            culture_req = ALL_CULTURE_TRAITS[data[3]]
        except KeyError:
            if len(data[3]) != 0:
                print("ERROR: Missing Culture trait: " + str(data[3]))
            culture_req = None
        pops = data[4]
        try:
            resource_req = data[5]
        except IndexError:
            resource_req = None
        b = Building(Name, land_req, type, culture_req, pops, resource_req)
        ALL_BUILDINGS[Name] = b
        i += 1

    i = 0
    for data in outputs:
        Name = data[0]
        b = ALL_BUILDINGS[Name]
        b.output["Food"] = int(data[1])
        b.output["Iron"] = int(data[2])
        b.output["Textiles"] = int(data[3])
        b.output["Wood"] = int(data[4])
        b.output["TradeGoods"] = int(data[5])
        b.output["LuxuryGoods"] = int(data[6])
        b.output["Equipment"] = int(data[7])
        b.output["Horses"] = int(data[8])
        b.output["Camels"] = int(data[9])
        b.output["Elephants"] = int(data[10])
        b.output["Stone"] = int(data[11])
        b.output["PreciousMaterials"] = int(data[12])
    print("Read " + str(len(ALL_BUILDINGS.keys())) + " Buildings.")

def readSettlements():
    settlements = open( "World/Settlements.tsv","r")
    i = 0
    for line in settlements:
        if i != 0:
            line = line.replace("\n","")
            data = line.split("\t")
            region_name = data[0]
            name = data[1]
            size = data[2]
            type = data[3]
            rank = data[4]
            xpos = data[5]
            ypos = data[6]
            s = Settlement(region_name, name, size, type, rank, xpos, ypos)
            ALL_SETTLEMENTS[region_name] = s
            SETTLED_REGIONS[region_name] = s
        i += 1
    settlements.close()
    print("Loaded " + str(i) + " Settlements. ")

def readResources():
    resources = loadTsvFile( "World/Resources.tsv")
    resourceToRegion = loadTsvFile( "World/resourceToRegion.tsv")
    i = 0
    for data in resources:
        name = data[0]
        min_prec = data[1]
        max_prec = data[2]
        min_temp = data[3]
        max_temp = data[4]
        priority = data[5]
        purpose  = data[6]
        r = Resource(name, min_prec, max_prec, min_temp, max_temp, priority, purpose)
        ALL_RESOURCES[name] = r
        i += 1
    for data in resourceToRegion:
        region = data[0]
        r_name = data[1]
        amount = data[2]
        resource = ALL_RESOURCES[r_name]
        settlement = ALL_SETTLEMENTS[region]
        settlement.minerals[resource] = int(amount)

    print("Read " + str(i) + " resources")

def readPopulations():
    resources = open( "World/Populations.tsv","r")
    i = 0
    for line in resources:
        if i != 0:
            line = line.replace("\n","")
            data = line.split("\t")
            name = data[0]
            occupation = data[1]
            prestige = data[2]
            food_demand = data[3]
            trade_good_demand = data[4]
            luxury_good_demand = data[5]
            p = Population(name, occupation, prestige, food_demand, trade_good_demand, luxury_good_demand)
            ALL_POPULATIONS[name] = p
            OCCUPATION_TO_POP[occupation] = p
        i += 1
    print("Read " + str(i) + " populations. ")
    resources.close()

def readNomadRegions():
    regions = loadTsvFile("World/NomadRegions.tsv")
    i = 0
    for data in regions:
        n = Region(data[0], data[1], data[2])
        NOMAD_REGIONS[data[0]] = n
        ALL_SETTLEMENTS[data[0]] = n
        i += 1
    print("Read " + str(i) + " nomad regions. ")

def getDistance(hub, node):
    x_distance = abs(hub.xpos - node.xpos)
    y_distance = abs(hub.ypos - node.ypos)
    sqr_distance = x_distance **2 + y_distance **2
    return sqr_distance

# Autogenerate settlement connections by finding out the distance between all settlements
def connectSettlements():
    for hub in ALL_SETTLEMENTS.values():
        for node in ALL_SETTLEMENTS.values():
            if getDistance(hub, node) <= MAX_CONNECT_DISTANCE_LAND:
                hub.connectToSettlement(node)


    for hub in ALL_SETTLEMENTS.values():
        l = len(hub.adjacentSettlements)
        if l == 0:
            print(hub.region_name + " has 0 connections.")
        elif l > 6:
            print(hub.region_name + " has " + str(l) + " connections!")

# Read settlement connections from text files
def loadSettlementConnections():
    connections = loadTsvFile("World/SettlementConnections.tsv")
    for data in connections:
        r1 = ALL_SETTLEMENTS[data[0]]
        r2 = ALL_SETTLEMENTS[data[1]]
        type = data[3]
        r1.connectToSettlement(r2, type)

    for hub in ALL_SETTLEMENTS.values():
        l = len(hub.adjacentSettlements)
        if l == 0:
            print(hub.region_name + " has 0 connections.")
        elif l > 6:
            print(hub.region_name + " has " + str(l) + " connections!")

def writeSettlementConnections():
    connections = open("World/SettlementConnections.tsv", "w")
    connections.write("RegionA\tRegionB\n")
    i = 0
    writtenPairs = []
    for s in NOMAD_REGIONS.values():
        if len(s.adjacentSettlements) == 0:
            connections.write(s.region_name + "\tNOTHING\n")
        for c in s.adjacentSettlements:
            if c.type == "nomadic":
                oppositePair = c.region_name + s.region_name
                pair = s.region_name + c.region_name
                if pair not in writtenPairs and oppositePair not in writtenPairs:
                    line = s.region_name + "\t" + c.region_name + "\n"
                    connections.write(line)
                    writtenPairs.append(pair)
                    i += 1
    print("Wrote " + str(i) + " settlement connections. ")
    connections.close()

def getAverageTerrain(regions):
    temp = 0
    precipitation = 0
    fields = 0
    woods = 0
    hills = 0
    mountains = 0
    deserts = 0
    wetlands = 0
    for region in regions:
        temp += region.terrain.temp
        precipitation += region.terrain.prec
        fields += region.terrain.fields
        woods += region.terrain.woods
        hills += region.terrain.hills
        mountains += region.terrain.mountains
        deserts += region.terrain.deserts
        wetlands += region.terrain.wetlands

    land_divisor = float(len(regions))
    climate_divisor = float(len(regions))
    average = Terrain("Average Terrain", temp/climate_divisor,
                    precipitation/climate_divisor, fields/land_divisor,
                    woods/land_divisor, hills/land_divisor, mountains/land_divisor,
                     deserts/land_divisor, wetlands/land_divisor)
    return average

def loadCultures():
    cultures = loadTsvFile("World/Cultures.tsv")
    traits = loadTsvFile("World/CultureTraits.tsv")
    culture_to_settlement = loadTsvFile("World/CultureToSettlements.tsv")
    culture_to_trait = loadTsvFile("World/CultureToTraits.tsv")
    for data in cultures:
        c = Culture(data[0], data[1], data[2])
        ALL_CULTURES[data[0]] = c

    for data in traits:
        t = Trait(data[0],data[1],data[2],data[3],data[4],data[5])
        ALL_CULTURE_TRAITS[data[0]] = t

    for data in culture_to_settlement:
        try:
            s = ALL_SETTLEMENTS[data[0]]
            c = ALL_CULTURES[data[1]]

            s.culture = c
            c.settlements.append(s)
        except KeyError as e:
            print(e)
            print("Cannot find settlement \'" + str(data[0]) + "\' and culture \'" + str(data[1]) + "\'")

    for data in culture_to_trait:
        culture_name = data[0]
        trait_name = data[1]
        try:
            c = ALL_CULTURES[culture_name]
            try:
                t = ALL_CULTURE_TRAITS[trait_name]
                c.addTrait(t)
            except KeyError:
                print("Error finding trait \'" + str(trait_name)+ "\'")
        except KeyError:
            print("Error finding culture \'" + str(culture_name) + "\'")

def write_implicit_culture_traits():
    implicit_traits = open("World/ImplicitTraits.tsv","w")
    implicit_traits.write("Culture\tTrait\n")
    i = 0
    for key in ALL_CULTURES.keys():
        culture = ALL_CULTURES[key]
        for trait in culture.implicit_traits:
            implicit_traits.write(culture.name + "\t" + trait.name + "\n")
            i += 1
    print("Wrote " + str(i) + " implicit traits.")
    implicit_traits.close()

def setValidResources():
    for key in ALL_SETTLEMENTS.keys():
        settlement = ALL_SETTLEMENTS[key]

        # Add ports for river and coast settlements
        if settlement.type in ["Coastal", "Lake side", "River crossing"]:
            settlement.ports = settlement.getRankScore()
        else:
            settlement.ports = 0

        # make sure ports are counted
        settlement.setLandRemaining()

        # Determine what crops grow in the region
        settlement.valid_resources = []
        for r_name in ALL_RESOURCES.keys():
            resource = ALL_RESOURCES[r_name]
            if settlement.terrain.isResourceValid(resource):
                settlement.valid_resources.append(resource)
            else:
                pass

# Look at all of the economic output in these regions and total them up to see
# whats there, whats missing, etc.
def getNetMarket(regions):
    word = ""
    marketSupply = dict()
    for region in regions:
        supply = region.getSupplyDemand()
        for key in supply.keys():
            val = supply[key]
            try:
                marketSupply[key] += val
            except KeyError:
                marketSupply[key] = val

    for key in marketSupply.keys():
        word += ("\t* " + str(key) + ": " + str(marketSupply[key])) + "\n"
    return word

def generateBuildings():
    # Do the initial setup before iterating
    setValidResources()

    settlements_left = len(ALL_SETTLEMENTS)
    ITERS = 60
    debug = False
    # grow the settlements by iteratively adding new valid buildings to them
    for i in range(ITERS):
        if debug:
            print("#" * 100)
            print("Starting iteration: " + str(i))
        for region in ALL_SETTLEMENTS.values():
            valid_buildings = []
            if debug:
                print("Checking region " + region.region_name)
                regionsupply = region.getSupplyDemand()
                print("Found " + region.region_name + " supply to be: ")
                for r in regionsupply.keys():
                    print("\t- " + str(r) + "\t" + str(regionsupply[r]))
                print("Population: " + str(region.getPopulation()))
            for building in ALL_BUILDINGS.values():
                if region.isBuildingAllowed(building):
                    valid_buildings.append(building)

            if len(valid_buildings) == 0:
                if debug:
                    print("Region " + region.region_name + " has no valid buildings to add.")
                if i == ITERS - 1:
                    settlements_left -= 1
            else:
                highest_utility = 0
                best_building = None
                for building in valid_buildings:
                    utility = building.calculateUtility(region, debug)
                    #print("\t* " + str(building) + " has utility "+ str(utility))
                    if utility > highest_utility:
                        highest_utility = utility
                        best_building = building

                if best_building == None:
                    if debug:
                        print("Region " + region.region_name + " has no buildings with positive utility.")
                    if i == ITERS - 1:
                        settlements_left -= 1
                else:
                    if debug:
                        print("Adding best building: " + str(best_building))
                    region.addBuilding(best_building)

            # Start next iteration
            if debug:
                break
    print("There are " + str(settlements_left) + " settlements that are not out of buildings. ")

def evaluateCultureTotals():
    for culture in ALL_CULTURES.values():
        print("-"*100)
        print(culture.getCultureDescription())


def getBuildingTotals(regions, printZeroes=False):
    totals = dict()
    word = ""
    for b in ALL_BUILDINGS.keys():
        totals[b] = 0
    for set in regions:
        for building in set.buildings.keys():
            num = set.buildings[building]
            try:
                totals[building] += num
            except KeyError:
                totals[building] = num
    for b in totals.keys():
        v = totals[b]
        if v == 0 :
            if printZeroes:
                word += ("-->\t* " + str(b) + ": " + str(v) + "\n")
        else:
            word += ("   \t* " + str(b) + ": " + str(v) + "\n")
    return word

def evaluateProvinces():
    for province in PROVINCE_TO_REGIONS.keys():
        regions = PROVINCE_TO_REGIONS[province]
        print("*" * 100)
        print("\n" + str(province) + " regions market: ")
        print(getNetMarket(regions))

    print("#" * 100 + "\nNomad regions market: ")
    print(getNetMarket(NOMAD_REGIONS.values()))

    print("#" * 100 + "\nSettled regions market: ")
    print(getNetMarket(SETTLED_REGIONS.values()))

    print("#" * 100 + "\nGlobal market:")
    print(getNetMarket(ALL_SETTLEMENTS.values()))

    print("#" * 100 + "\nBuilding totals:")
    getBuildingTotals(ALL_SETTLEMENTS.values())

def main():
    readSettlements()
    readNomadRegions()
    loadDefaultProvinceTerrain()
    loadSettlementTerrain()
    readPopulations()
    loadCultures()
    readResources()
    loadBuildings()
    generateBuildings()
    writeBuildings()
    evaluateCultureTotals()

    #evaluateProvinces()

# Run main()
main()

# EOF
