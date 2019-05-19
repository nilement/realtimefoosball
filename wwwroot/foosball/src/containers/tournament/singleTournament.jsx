import React from 'react';
import axios from 'axios';
import LoadingIndicator from 'components/LoadingIndicator';
import PlayoffBracket from 'components/PlayoffBracket';
import GroupTable from 'components/GroupTable';
import { UserContext } from 'containers/App/userContext';

const numberOfRounds = n => { 
    let power = 1;
    var cnt=0;
    while(n > 0){
        n -= power;
        power *= 2;
        cnt++;
    }; 
    return cnt
}

const calculateRounds = (matchups) => {
    let roundsCount = numberOfRounds(matchups.length);
    let rounds = [];
    let i = 0;
    let games = 1;
    let node = 0;
    while (i < roundsCount){
        let round = [];
        for(var j = 0; j < games; j++){
            round.push(matchups[node]);
            node++;
        }
        rounds.push(round);
        i++;
        games *= 2;
    }
    return rounds.reverse();
}

const getRandomInt = max => {
    return Math.floor(Math.random() * Math.floor(max));
  }

export default class SingleTournament extends React.Component{
    constructor(){
        super();
        this.advanceToPlayoffs = this.advanceToPlayoffs.bind(this);
        this.playMatchup = this.playMatchup.bind(this);
    }

    state = {
        loading: false,
        error: false,
        tournament: false,
    }

    getTournamentInfo(token){
        axios.get('/api/tournament/' + this.props.match.params.tournamentId, { headers: { 'Authorization': 'Bearer ' + token }})
        .then((response) => {
            this.setState({ loading: false, tournament: response.data, rounds: calculateRounds(response.data.matchups) });
        })
        .catch((err) => {
            this.setState({ loading: false, error: err });
        })
    }

    advanceToPlayoffs(tournamentPlayerId){
        var obj = {
            tournamentPlayerId: tournamentPlayerId
        };
        axios.post('/api/tournament/moveToPlayoffs', obj)
            .then((response) => {
                
            })
            .catch(err => {
                console.log('well, fuck')
            })
    }

    playMatchup(matchup){
        var obj = {
            matchupId: matchup.id
        }
        var dice = getRandomInt(2);
        if (dice){
            obj.bluePlayer = matchup.t1P1.player.id;
            obj.redPlayer = matchup.t2P1.player.id;
        } else {
            obj.redPlayer = matchup.t1P1.player.id
            obj.bluePlayer = matchup.t2P1.player.id
        }
        axios.post('/api/game/playoffsgame', obj)
            .then((response) => {
                this.props.history.push("/game/" + response.data.id);
            })
            .catch(err => {
                console.log('well, fuck')
            })
    }

    render(){
        if (this.state.loading) return <LoadingIndicator />
        if (this.state.error) return <div>Well, fuck.</div>
        let { name, startDate } = this.props;
        let { tournament, rounds } = this.state;
        return(
            <UserContext.Consumer>
            {({ authed, token }) => {
                if (!token){
                    return <div>Need to be authed</div>;
                }
                if (token && !this.state.tournament) {
                    this.getTournamentInfo(token);
                    return <LoadingIndicator />;
                }
                return (
                    <div>
                        <div>Tournament {tournament.tournament.name}</div>
                        <div>Start date: {tournament.tournament.startDate}</div>
                        {tournament.matchups.filter(x => x.t1P1 !== null && x.t2P1 !== null).map((matchup, index) => (
                            <>
                                <h2>{matchup.t1P1.player.name} vs {matchup.t2P1.player.name}</h2>
                                <button onClick={() => this.playMatchup(matchup)}>Play matchup</button>
                            </>
                        ))}
                        {tournament.groups.map((group, index) => (
                            <div key={index}>
                                <h1>Group #{group.number}</h1>
                                <GroupTable group={group.tournamentPlayers} advance={this.advanceToPlayoffs} />
                                {/* {group.tournamentPlayers.map((player, index) => (
                                <>
                                    <h2>{player.player.name}</h2>
                                    <span>Wins: {player.wins} Losses: {player.losses}</span>
                                </>
                                ))} */}
                                {/* <span>{group}</span> */}
                                {/* <span>{tournament.startDate} </span>
                                <span>{tournament.groupCount}</span>
                                <button>Tournament info</button> */}
                            </div>
                        ))}
                        <PlayoffBracket rounds={rounds}/>
                    </div>
                )
            }}
            </UserContext.Consumer>
        )
    }
}
