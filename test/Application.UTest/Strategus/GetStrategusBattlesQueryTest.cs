﻿using System.Threading;
using System.Threading.Tasks;
using Crpg.Application.Strategus.Queries;
using Crpg.Domain.Entities;
using Crpg.Domain.Entities.Strategus;
using Crpg.Domain.Entities.Strategus.Battles;
using Crpg.Domain.Entities.Users;
using NUnit.Framework;

namespace Crpg.Application.UTest.Strategus
{
    public class GetStrategusBattlesQueryTest : TestBase
    {
        [Test]
        public async Task ShouldGetBattlesMatchingThePhases()
        {
            StrategusBattle[] battles =
            {
                new()
                {
                    Region = Region.NorthAmerica,
                    Phase = StrategusBattlePhase.Hiring,
                    Fighters =
                    {
                        new StrategusBattleFighter
                        {
                            Side = StrategusBattleSide.Attacker,
                            Commander = true,
                            Hero = new StrategusHero { Troops = 20, User = new User() },
                        },
                        new StrategusBattleFighter
                        {
                            Side = StrategusBattleSide.Attacker,
                            Commander = false,
                            Hero = new StrategusHero { Troops = 15, User = new User() },
                        },
                        new StrategusBattleFighter
                        {
                            Side = StrategusBattleSide.Defender,
                            Commander = false,
                            Hero = new StrategusHero { Troops = 35, User = new User() },
                        },
                        new StrategusBattleFighter
                        {
                            Side = StrategusBattleSide.Defender,
                            Commander = true,
                            Hero = new StrategusHero { Troops = 10, User = new User() },
                        },
                    },
                },
                new()
                {
                    Region = Region.NorthAmerica,
                    Phase = StrategusBattlePhase.Battle,
                    Fighters =
                    {
                        new StrategusBattleFighter
                        {
                            Side = StrategusBattleSide.Attacker,
                            Commander = true,
                            Hero = new StrategusHero { Troops = 100, User = new User() },
                        },
                        new StrategusBattleFighter
                        {
                            Side = StrategusBattleSide.Defender,
                            Commander = true,
                            Settlement = new StrategusSettlement
                            {
                                Name = "toto",
                                Troops = 12,
                            },
                        },
                        new StrategusBattleFighter
                        {
                            Side = StrategusBattleSide.Defender,
                            Commander = false,
                            Hero = new StrategusHero { Troops = 35, User = new User() },
                        },
                    },
                },
                new() { Region = Region.NorthAmerica, Phase = StrategusBattlePhase.Preparation },
                new() { Region = Region.Europe, Phase = StrategusBattlePhase.Hiring },
                new() { Region = Region.Asia, Phase = StrategusBattlePhase.Battle },
                new() { Region = Region.NorthAmerica, Phase = StrategusBattlePhase.End },
            };
            ArrangeDb.StrategusBattles.AddRange(battles);
            await ArrangeDb.SaveChangesAsync();

            GetStrategusBattlesQuery.Handler handler = new(ActDb, Mapper);
            var res = await handler.Handle(new GetStrategusBattlesQuery
            {
                Region = Region.NorthAmerica,
                Phases = new[] { StrategusBattlePhase.Hiring, StrategusBattlePhase.Battle },
            }, CancellationToken.None);

            Assert.IsNull(res.Errors);

            var battlesVm = res.Data!;
            Assert.AreEqual(2, battlesVm.Count);

            Assert.AreEqual(Region.NorthAmerica, battlesVm[0].Region);
            Assert.AreEqual(StrategusBattlePhase.Hiring, battlesVm[0].Phase);
            Assert.IsNotNull(battlesVm[0].Attacker);
            Assert.IsNotNull(battlesVm[0].Attacker.Hero);
            Assert.AreEqual(35, battlesVm[0].AttackerTotalTroops);
            Assert.IsNotNull(battlesVm[0].Defender);
            Assert.IsNotNull(battlesVm[0].Defender!.Hero);
            Assert.AreEqual(45, battlesVm[0].DefenderTotalTroops);

            Assert.AreEqual(Region.NorthAmerica, battlesVm[1].Region);
            Assert.AreEqual(StrategusBattlePhase.Battle, battlesVm[1].Phase);
            Assert.IsNotNull(battlesVm[1].Attacker);
            Assert.IsNotNull(battlesVm[1].Attacker.Hero);
            Assert.AreEqual(100, battlesVm[1].AttackerTotalTroops);
            Assert.AreEqual(47, battlesVm[1].DefenderTotalTroops);
            Assert.IsNotNull(battlesVm[1].Defender);
            Assert.IsNotNull(battlesVm[1].Defender!.Settlement);
        }
    }
}
