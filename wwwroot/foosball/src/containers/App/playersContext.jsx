import React from 'react';
import axios from 'axios';

export const PlayersContext = React.createContext({
    loading: false,
    errors: null,
    players: []
});

export class PlayerProvider extends React.Component{
    constructor(){
        super();
        this.state = {
            loading: false,
            errors: null,
            players: []
        }
    }

    componentDidMount(){
        this.setState({
            loading: true
        })
        axios.get('/api/players')
            .then((response) => {
                this.setState({
                    loading: false,
                    players: response.data
                })
            })
            .catch((error) => {
                this.setState({
                    loading: false,
                    error: error
                });
            });
    }

    render() {
        const { children } = this.props;
        return(
            <PlayersContext.Provider value={this.state}>{children}</PlayersContext.Provider>
        );
    }
}
