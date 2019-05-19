import React from 'react';
import axios from 'axios';

export const TournamentsContext = React.createContext({
    loading: false,
    errors: null,
    tournaments: []
});

export class TournamentsProvider extends React.Component{
    constructor(){
        super();
        this.state = {
            loading: false,
            errors: null,
            tournaments: []
        }
    }

    componentDidMount(){
        this.setState({
            loading: true
        })
        axios.get('/api/tournament/running')
            .then((response) => {
                this.setState({
                    loading: false,
                    tournaments: response.data
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
            <TournamentsContext.Provider value={this.state}>{children}</TournamentsContext.Provider>
        );
    }
}
