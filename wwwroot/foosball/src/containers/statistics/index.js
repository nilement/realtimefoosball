import React from 'react';
import Goalscorers from '../../components/Goalscorers';
import axios from 'axios';

export default class Statistics extends React.Component{
    state =  {
        goalscorers: false,
        loading: false,
        error: false,
    }


    componentDidMount(){
        axios.get('/api/statistics/mostgoals')
            .then((resp) => {
                this.setState({ goalscorers: resp.data, loading: false, error: false })
            })
            .catch((err) => {
                this.setState({ loading: false, error: err });
            })
    }

    render(){
        return(
            <>
                {this.state.goalscorers && <Goalscorers data={this.state.goalscorers}/>}
                <div>Statistics</div>
            </>
        );
    }
}