import React from 'react';

export class Login extends React.Component{
    componentDidMount(){
        if (this.props.match.params.token && !this.props.authed){
            this.props.login(this.props.match.params.token);
        }
    }

    render(){
        return(
            <>
            </>
        );
    }
}