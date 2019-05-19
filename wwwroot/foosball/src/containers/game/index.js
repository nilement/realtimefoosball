import React from 'react';
import axios from 'axios';

export default class Game extends React.Component{
    constructor(){
        super();
        this.addBlueGoal = this.addBlueGoal.bind(this);
        this.addRedGoal = this.addRedGoal.bind(this);
        this.closeTracking = this.closeTracking.bind(this);
    }

    state = {
        loaded: false,
        redGoals: 0,
        blueGoals: 0,
    }

    componentDidMount(){
        if (this.props.location.state){
            this.setState({
                game: this.props.location.state.gameInfo
            })
        } else {
            axios.get('/api/game/' + this.props.match.params.gameId)
            .then( response => {
                this.setState({
                    game: response.data
                })
            })
            .catch(err => {

            })
        }
        (() => {
          var socket = new WebSocket('ws://localhost:8001');
          socket.onmessage = (m) => {
            console.log(m);
            var data = JSON.parse(m.data);
            if (data){
              if (data.event == 'key accepted'){
                  this.setState({ 
                      socket: socket
                  })
                  socket.send('track')
              }
              if (data.event === 'G'){
                if (data.side === 'R'){
                  this.addRedGoal();
                } else {
                  this.addBlueGoal();
                }
              }
            }
          }
          socket.onopen = function() {
            socket.send('key');
          }
        })();
    }

    closeTracking(){
        this.state.socket.send('stop');
    }

    finishGame(){
        axios.post('/api/game/finish', { 
            BlueScore: this.state.blueGoals,
            RedScore: this.state.redGoals,
            gameId: this.props.match.params.gameId
        })
        .then(resp => {
            this.props.history.push('/gameInfo/' + this.props.match.params.gameId);
        })
        .catch(err => {

        })
    }

    addRedGoal(){
        if (this.state.redGoals === 9){
            this.setState({ redGoals: this.state.redGoals + 1});
            this.finishGame();
        } else {
            this.setState({ redGoals: this.state.redGoals + 1});
        }
    }

    addBlueGoal(){
        if (this.state.blueGoals === 9){
            this.setState({ blueGoals: this.state.blueGoals + 1});
            this.finishGame();
        } else {
            this.setState({ blueGoals: this.state.blueGoals + 1});
        }
    }

    render(){
        let { redTeam, blueTeam } = this.props;
        return(
            <>
                <h1>Game # {this.props.match.params.gameId}</h1>
                <h3>Blue {this.state.blueGoals} vs. {this.state.redGoals} Red</h3>
                <button onClick={this.addBlueGoal}>Add blue goals</button>
                <button onClick={this.addRedGoal}>Add red goals</button>
            </>
        )
    }
};