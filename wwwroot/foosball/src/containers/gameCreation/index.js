import React from 'react';
import axios from 'axios';

import { PlayersContext } from 'containers/App/playersContext'
import { TournamentsContext } from 'containers/App/tournamentsContext';
import PlayersGrid from 'components/PlayersGrid';

const getIds = players =>{
    return players.map(player => player.id);
};

export default class GameSelection extends React.Component{
    constructor(){
        super();
        this.addToTeam = this.addToTeam.bind(this);
        this.createGame = this.createGame.bind(this);
        this.removeBluePlayer = this.removeBluePlayer.bind(this);
        this.removeRedPlayer = this.removeRedPlayer.bind(this);
        this.selectTournament = this.selectTournament.bind(this);
    }

    state = {
        blueTeamPlayers: [],
        redTeamPlayers: []
    }

    addToTeam(player){
        let { redTeamPlayers, blueTeamPlayers } = this.state
        if (blueTeamPlayers.length + redTeamPlayers.length >= 4){
            return;
        }
        if (blueTeamPlayers.length <= redTeamPlayers.length){
            this.setState({ blueTeamPlayers: [...blueTeamPlayers, player]})
        } else {
            this.setState({ redTeamPlayers: [...redTeamPlayers, player]});
        }
    }

    removeBluePlayer(removePlayer){
        this.setState({ blueTeamPlayers: this.state.blueTeamPlayers.filter(player => { 
            return player !== removePlayer 
        })});
    }

    removeRedPlayer(removePlayer){
        this.setState({ redTeamPlayers: this.state.redTeamPlayers.filter(player => { 
            return player !== removePlayer 
        })});
    }

    selectTournament(event){
        this.setState({ tournament: event.target.value })
    }

    createGame(){
        let { blueTeamPlayers, redTeamPlayers } = this.state;
        let link = '/api/game';
        let obj = {
            BlueTeam: getIds(blueTeamPlayers),
            RedTeam: getIds(redTeamPlayers),
            Type: blueTeamPlayers.length + redTeamPlayers.length > 2 ? 1 : 0
        };
        if (this.state.tournament){
            link = '/api/game/tournamentGame';
            obj.tournamentId = this.state.tournament;
            obj.BlueTeam = this.state.blueTeamPlayers[0].id;
            obj.RedTeam = this.state.redTeamPlayers[0].id;
        }
        axios.post(link, obj)
        .then(response => {
            this.props.history.push({ pathname:"/game/" + response.data.id, state: { gameInfo: response.data }});
        })
        .catch(err => {
            this.setState({ error: err });
        })
    }

    render(){
        return(
            <PlayersContext.Consumer>
                {({ loading, error, players }) => {
                    if (loading) return <div>fuck</div>;
                    if (error) return <div>error</div>;
                    return(
                        <TournamentsContext.Consumer>
                            {({ loading, error, tournaments }) => {
                                return(
                                    <>
                                        <PlayersGrid players={players} addFunction={this.addToTeam}/>
                                        <h1>Blue team:</h1>
                                        {this.state.blueTeamPlayers.map((player, index) => (
                                            <div>
                                                <span>{player.name} {player.wins} - {player.losses}</span>
                                                <button onClick={() => this.removeBluePlayer(player)}>Remove player</button>
                                            </div>
                                        ))}
                                        <h1>Red team:</h1>
                                        {this.state.redTeamPlayers.map((player, index) => (
                                            <div>
                                                <span>{player.name} {player.wins} - {player.losses}</span>
                                                <button onClick={() => this.removeRedPlayer(player)}>Remove player</button>
                                            </div>
                                        ))}
                                        <button onClick={this.createGame}>
                                            Start game
                                        </button>
                                        <select value={this.state.tournament} onChange={this.selectTournament}>
                                            <option>None</option>
                                            {tournaments.map((tournament, index) => (
                                                <option value={tournament.id}>{tournament.name}</option>
                                            ))}
                                        </select>
                                    </>
                                )}}
                        </TournamentsContext.Consumer>
                    )
                }}
            </PlayersContext.Consumer>
        );
    }
}