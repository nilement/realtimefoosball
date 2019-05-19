import React from 'react';
import axios from 'axios';

export default class TournamentCreation extends React.Component{
    constructor(){
        super();
        this.changeNumberOfGroups = this.changeNumberOfGroups.bind(this);
        this.addPlayer = this.addPlayer.bind(this);
        this.removePlayer = this.removePlayer.bind(this);
        this.submitTournament = this.submitTournament.bind(this);
    }

    state = {
        tournamentName: '',
        numberOfGroups: '',
        tournamentPlayers: [],
    }

    
    changeNumberOfGroups(event){
        this.setState({ numberOfGroups: event.target.value });
    }

    addPlayer(player){
        this.setState({ tournamentPlayers: [...this.state.tournamentPlayers, player ]})
    }

    removePlayer(deletedPlayer){
        this.setState({ tournamentPlayers: this.state.tournamentPlayers.filter(player => { 
            return player !== deletedPlayer 
        })});
    }

    submitTournament(event){
        event.preventDefault();
        let header = { headers: { Authorization: 'Bearer ' + localStorage.getItem("token")}}
        axios.post('/api/tournament', {
            Tournament: {
                name: this.state.tournamentName,
                groupCount: this.state.numberOfGroups,
                hasGroupStage: true,
            },
            Players: this.state.tournamentPlayers.map(x => x.id)
        }, header)
        .then(response => {
            console.log('okay');
        })
        .catch(error => {
            console.log('no');
        })
    }

    render(){
        let { players } = this.props;
        return(
            <>
                <label>Tournament's name </label>
                <input onChange={(event) => this.setState({ tournamentName: event.target.value })}></input>
                <label>Number of groups</label>
                <select value={this.state.numberOfGroups} onChange={this.changeNumberOfGroups}>
                    <option value="1">1</option>
                    <option value="2">2</option>
                    <option value="4">4</option>
                </select>
                <div>
                    <span>Available players:</span>
                    {players.map( (player, index) => (
                        <div key={index}>
                            {this.state.tournamentPlayers.indexOf(player) < 0 &&
                            <div>
                                <span>{player.name}</span>
                                <button onClick={() => this.addPlayer(player)}>Add to tournament</button>
                            </div>
                            }
                        </div>
                    ))}
                </div>
                <div>
                    <span>Added players:</span>
                    {this.state.tournamentPlayers.map((player, index) => (
                        <div key={index+100}>
                            <span>{player.name}</span>
                            <button onClick={() => this.removePlayer(player)}>Remove from tournament</button>
                        </div>
                    ))}
                </div>
                <button onClick={this.submitTournament}>Submit tournament</button>
            </>
        )
    }
}