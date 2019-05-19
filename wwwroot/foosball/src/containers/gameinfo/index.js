import React from 'react';
import axios from 'axios';
import LoadingIndicator from 'components/LoadingIndicator';
import { format, formatDistanceStrict } from 'date-fns';

export default class GameInfo extends React.Component{
    state = {
        loading: true,
    }

    componentDidMount(){
        this.setState({
            loading: true
        });
        axios.get('/api/game/' + this.props.match.params.gameId)
        .then( response => {
            const startDate = Date.parse(response.data.startDate);
            const endDate = Date.parse(response.data.endDate);
            const durationFormated = formatDistanceStrict(endDate, startDate);
            const startDateFormated = format(startDate, 'yyyy-MM-dd HH:mm');
            this.setState({
                game: {
                    ...response.data,
                    startDateFormated: startDateFormated,
                    durationFormated: durationFormated
                },
                loading: false
            })
        })
    }

    render(){
        if (this.state.loading){
            return <LoadingIndicator />
        }
        return(
            <div>
                <h1>Game # {this.props.match.params.gameId}</h1>
                <h2>Start date: {this.state.game.startDateFormated} </h2>
                <h3>Blue {this.state.game.bP1.name} {this.state.game.blueTeamScore} vs. {this.state.game.redTeamScore} {this.state.game.rP1.name} Red</h3>
                <h2>Playing time: {this.state.game.durationFormated} </h2>
            </div>
        )
    }
}