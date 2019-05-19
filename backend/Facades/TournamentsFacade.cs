using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ToughBattle.Controllers.Dto;
using ToughBattle.Database;
using ToughBattle.Models;
using ToughBattle.Models.Enums;

namespace ToughBattle.Facades
{
    public class TournamentsFacade : ITournamentsFacade
    {
        private readonly FoosballContext _db;

        public TournamentsFacade(FoosballContext ctx)
        {
            _db = ctx;
        }

        public async Task<List<Tournament>> RunningTournaments()
        {
            return await _db.Tournaments.Where(x => !x.HasEnded).ToListAsync();
        }

        public async Task<TournamentInfo> TournamentDetails(int tournamentId)
        {
            return new TournamentInfo()
            {
                Groups =  await RetrieveGroups(tournamentId),
                Tournament = await _db.Tournaments.FirstOrDefaultAsync(y => y.Id == tournamentId),
                Matchups = await RetrieveTournamentMatchups(tournamentId)
            };
        }

        public async Task<List<TournamentPlayer>> TournamentsPlayers(int tournamentId)
        {
            return await _db.TournamentPlayers.Where(x => x.Tournament.Id == tournamentId).ToListAsync();
        }

        public async Task<List<TournamentGroup>> TournamentGroups(int tournamentId)
        {
            return await RetrieveGroups(tournamentId);
        }

        public async Task<TournamentInfo> CreateTournament(Tournament tournament, List<int> players)
        {
            tournament.StartDate = DateTime.Now;
            if (!tournament.HasGroupStage)
            {
                var value = PreviousPowerOf2(players.Count);
                if (value == 0)
                {
                    value = 1;
                }
                tournament.PlayoffMatchupsCount = value;
                tournament.PlayoffTree = Create1GroupTree(tournament.PlayoffMatchupsCount);
                var cnt = value - 1;
                var fst = true;
                foreach (var player in players)
                {
                    //                    var p = _db.Players.FirstOrDefault()
                    var tourneyPlayer = new TournamentPlayer();
                    var playerDb = await _db.Players.FirstOrDefaultAsync(x => x.Id == player);
                    tourneyPlayer.Player = playerDb;
                    tourneyPlayer.Tournament = tournament;
                    if (fst)
                    {
                        tournament.PlayoffTree[cnt].T1P1 = tourneyPlayer;
                        fst = false;
                    }
                    else
                    {
                        tournament.PlayoffTree[cnt].T2P1 = tourneyPlayer;
                        cnt--;
                        fst = true;
                    }
                }
                tournament.StartingPhase = tournament.PlayoffTree.Last().TournamentPhase;
                await _db.AddAsync(tournament);
                await _db.SaveChangesAsync();
                return new TournamentInfo {Tournament = tournament};
            }
            tournament.PlayoffMatchupsCount = PreviousPowerOf2(players.Count) / 2;
            if (tournament.GroupCount == 1)
            {
                tournament.PlayoffTree = Create1GroupTree(tournament.PlayoffMatchupsCount);
            } else if (tournament.GroupCount == 2)
            {
                tournament.PlayoffTree = Create2GroupsTree(tournament.PlayoffMatchupsCount);
            }
            else
            {
                throw new NotImplementedException();
            }

            tournament.StartingPhase = tournament.PlayoffTree.Last().TournamentPhase;
            await _db.AddAsync(tournament);
            var list = await SeparateToGroups(tournament, players);
            foreach (var player in list)
            {
                await _db.AddAsync(player);
            }
            await _db.SaveChangesAsync();
            return new TournamentInfo {Groups = TrnGroups(tournament, list), Tournament = tournament};
        }

        public async Task<Matchup> PlayerToPlayoffs(int playerId)
        {
            var player = await _db.TournamentPlayers.Include(y => y.Tournament).Include(x => x.Tournament.PlayoffTree).FirstOrDefaultAsync(x => x.Id == playerId);
            var position = await CalculatePosition(player);
            var goThroughCount  =
                PreviousPowerOf2(_db.TournamentPlayers.Count(x => x.Tournament.Id == player.Tournament.Id)) / player.Tournament.GroupCount;
            if (position <= goThroughCount)
            {
                var tourney = _db.Matchups.Include(x => x.LowerPair).Include(y => y.UpperPair).Where(x => x.Tournament.Id == player.Tournament.Id);
                if (position > goThroughCount / 2 && player.Tournament.PlayoffTree.Count > 1)
                {
                    var lower = await tourney.FirstOrDefaultAsync(x =>
                        x.LowerPair.Group == player.GroupId && x.LowerPair.Place == position);
                    lower.T2P1 = player;
                    await _db.SaveChangesAsync();
                    return await tourney.FirstOrDefaultAsync();
                }

                Matchup matchup;
                matchup = await tourney.FirstOrDefaultAsync(x => x.UpperPair.Group == player.GroupId && x.UpperPair.Place == position && x.TournamentPhase == player.Tournament.StartingPhase);
                if (matchup != null)
                {
                    matchup.T1P1 = player;
                }
                else if (player.Tournament.StartingPhase == TournamentPhase.Final)
                {
                    matchup = await tourney.FirstOrDefaultAsync(x => x.LowerPair.Group == player.GroupId && x.LowerPair.Place == position && x.TournamentPhase == player.Tournament.StartingPhase);
                    matchup.T2P1 = player;
                }
                await _db.SaveChangesAsync();
                return await tourney.FirstOrDefaultAsync();
            }
            throw new ArgumentException();
        }

        private async Task<int> CalculatePosition(TournamentPlayer player)
        {
            var group = await _db.TournamentPlayers
                .Where(x => x.Tournament.Id == player.Tournament.Id && x.GroupId == player.GroupId)
                .OrderByDescending(x => x.Wins).ToListAsync();
            return group.FindIndex(x => x.Id == player.Id) + 1;
        }

        private async Task<List<Matchup>> RetrieveTournamentMatchups(int tournamentId)
        {
            return await _db.Matchups.Include(x => x.T1P1).Include(x => x.T2P1).Where(x => x.Tournament.Id == tournamentId).OrderBy(x => x.Id).ToListAsync();
        }

        private async Task<List<TournamentGroup>> RetrieveGroups(int tournamentId)
        {
            var list = new List<TournamentGroup>();
            var tournament = await _db.Tournaments.FirstOrDefaultAsync(x => x.Id == tournamentId);
            for (var i = 0; i < tournament.GroupCount; i++)
            {
                list.Add(new TournamentGroup { Number = i + 1, TournamentPlayers = new List<TournamentPlayer>() });
            }

            var players = _db.TournamentPlayers.Where(x => x.Tournament.Id == tournamentId).Include(x => x.Player);
            await players.ForEachAsync(x => list[x.GroupId].TournamentPlayers.Add(x));
            list.ForEach(x => x.TournamentPlayers = x.TournamentPlayers.OrderByDescending(y => y.Wins).ThenBy(z => z.Losses).ToList());
            return list;
        }

        private async Task<List<TournamentPlayer>> SeparateToGroups(Tournament tournament, List<int> players)
        {
            var random = new Random();
            var groupCnt = tournament.GroupCount;
            var max = players.Count / tournament.GroupCount;
            if (players.Count % tournament.GroupCount != 0)
            {
                max += 1;
            }
            var list = new List<TournamentPlayer>();
            var groups = new List<Group>();
            for (var i = 0; i < groupCnt; i++)
            {
                groups.Add(new Group {GroupNumber = i, PlayersCnt = 0});
            }

            foreach (var player in players)
            {
                var tourneyPlayer = new TournamentPlayer();
                var group = groups[random.Next(groups.Count)];
                var playerDb = await _db.Players.FirstOrDefaultAsync(x => x.Id == player);
                tourneyPlayer.Player = playerDb;
                tourneyPlayer.Tournament = tournament;
                tourneyPlayer.GroupId = group.GroupNumber;
                tourneyPlayer.Wins = 0;
                tourneyPlayer.Losses = 0;
                list.Add(tourneyPlayer);
                group.PlayersCnt++;
                if (group.PlayersCnt == max)
                {
                    groups.Remove(group);
                }
            }

            return list;
        }

        private List<TournamentGroup> TrnGroups(Tournament t, List<TournamentPlayer> pl)
        {
            List<TournamentGroup> tg = new List<TournamentGroup>();
            for (var i = 0; i < t.GroupCount; i++)
            {
                tg.Add(new TournamentGroup { Number = i + 1, TournamentPlayers = new List<TournamentPlayer>() });
            }

            pl.ForEach(x => tg[x.GroupId].TournamentPlayers.Add(x));
            tg.ForEach(x => x.TournamentPlayers = x.TournamentPlayers.OrderByDescending(y => y.Wins).ThenBy(z => z.Losses).ToList());
            return tg;
        }

        private bool CheckIfAdvancesToPlayoffs()
        {
            return true;
        }

//        private Matchup FirstPlayoffMatchup (FoosballContext db, TournamentPlayer player, int place, int groupQualifiers)
//        {
//            db.Matchups.Where(x => x.Tournament.Id == player.Tournament.Id)
//        }

//        public List<Matchup> MultiGroupTree(int groups, int groupQualifiers)
//        {
//            List<List<Matchup>> trees = new List<List<Matchup>>();
//            List<Matchup> merged = new List<Matchup>();
//            for (var i = 0; i < groups / 2; i++)
//            {
//                var tree = Create2GroupsTree(groupQualifiers, i * 2);
//                trees.Add(tree);
//            }
//
//            for (var i = 0; i < groups / 2; i += 2)
//            {
//                var first = trees[i];
//                var second = trees[i + 1];
//                var merge = new Matchup { TournamentPhase = TournamentPhase.Final, Merged = true};
//                IncrementPhases(first);
//                IncrementPhases(second);
//                first[0].NextMatchup = merge;
//                second[0].NextMatchup = merge;
//                merged.Add(merge);
//                for (var j = 0; j < groupQualifiers / 2; j++)
//                {
//
//                }
//            }
//        }

        public List<Matchup> Create1GroupTree(int groupQualifiers)
        {
            var list = new List<Matchup>();
            var matchups = 1;
            var height = 0;
            var node = 0;
            while (matchups <= groupQualifiers)
            {
                for (var i = 0; i < matchups; i++)
                {
                    if (height != 0)
                    {
                        var parent = PreviousLevel(node);
                        var parentMatchup = list[parent];
                        var upperPlayer = i % 2 == 0 ? parentMatchup.UpperPair : parentMatchup.LowerPair;
                        var lowerPlayer = new GroupPlacePair
                        {
                            Group = 0,
                            Place = GetOppositePosition(matchups * 2, upperPlayer.Place)
                        };
                        var m = new Matchup
                        {
                            UpperPair = upperPlayer,
                            LowerPair = lowerPlayer,
                            NextMatchup = list[parent],
                            TournamentPhase = (TournamentPhase)height,
                            //                            Tournament = t,
                            Wins1 = 0,
                            Wins2 = 0
                        };
                        list.Add(m);
                    }
                    else
                    {
                        var m = new Matchup
                        {
                            UpperPair = new GroupPlacePair { Group = 0, Place = 1 },
                            LowerPair = new GroupPlacePair { Group = 0, Place = 2 }
                        };
                        list.Add(m);
                    }

                    node++;
                }
                height++;
                matchups *= 2;
            }

            return list;
        }

        public List<Matchup> Create2GroupsTree (int groupQualifiers, int groupOffset = 0)
        {
            var list = new List<Matchup>();
            var levels = 1;
            var height = 0;
            var node = 0;
            while (levels <= groupQualifiers)
            {
                for (var i = 0; i < levels; i++)
                {
                    if (height != 0)
                    {
                        var parent = PreviousLevel(node);
                        var parentMatchup = list[parent];
                        var upperPlayer = i % 2 == 0 ? parentMatchup.UpperPair : parentMatchup.LowerPair;
                        var lowerPlayer = new GroupPlacePair
                        {
                            Group = GetOppositeGroup(upperPlayer.Group, 2) + groupOffset, Place = GetOppositePosition(levels, upperPlayer.Place)
                        };
                        var m = new Matchup
                        {
                            AdvanceToUpper = i % 2 == 0,
                            Finished = false,
                            UpperPair = upperPlayer,
                            LowerPair = lowerPlayer,
                            NextMatchup = list[parent],
                            TournamentPhase = (TournamentPhase) height,
//                            Tournament = t,
                            Wins1 = 0, Wins2 = 0
                        };
                        list.Add(m);
                    }
                    else
                    {
                        var m = new Matchup
                        {
                            Finished = false,
                            UpperPair = new GroupPlacePair {Group = 0 + groupOffset, Place = 1},
                            LowerPair = new GroupPlacePair {Group = 1 + groupOffset, Place = 1},
                            TournamentPhase =  TournamentPhase.Final
                        };
                        list.Add(m);
                    }

                    node++;
                }
                height++;
                levels *= 2;
            }

            return list;
        }

        private void IncrementPhases(List<Matchup> tree)
        {
            foreach (var matchup in tree)
            {
                matchup.TournamentPhase += 1;
            }
        }

        private void RandomizePlayers()
        {

        }

        private int PreviousLevel(int level)
        {
            if (level == 0) return 0;
            return (level - 1) / 2;
        }

        private int GetOppositePosition(int qualifiers, int place)
        {
            return 1 + qualifiers - place;
        }

        private int GetOppositeGroup(int groups, int groupN)
        {
            if (groupN == 1) return 0;
            if (groups % 2 == 0) return groups + 1;
            return groups - 1;
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        private int PreviousPowerOf2(int x)
        {
            x = x - 1;
            if (IsPowerOfTwo(x)) return x / 2;
            x = x | (x >> 1);
            x = x | (x >> 2);
            x = x | (x >> 4);
            x = x | (x >> 8);
            x = x | (x >> 16);
            return x - (x >> 1);
        }

        private class Group
        {
            public int GroupNumber;
            public int PlayersCnt;
        }
    }
}
