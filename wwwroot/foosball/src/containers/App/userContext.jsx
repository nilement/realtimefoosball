import React from 'react';

export const UserContext = React.createContext({
    authed: false,
    role: false,
    token: false,
});

export class UserProvider extends React.Component{
    constructor(){
        super();
        this.state = {
            authed: false,
            name: false,
            role: false,
            token: false,
            logIn: token => {
                localStorage.setItem('token', token);
                this.setState({ token: token, authed: true });
            }
        }
    }

    componentDidMount(){
        if (localStorage.getItem("token")){
            this.setState({ token: localStorage.getItem("token"), authed: true });
        }
    }

    render() {
        const { children } = this.props;
        return(
            <UserContext.Provider value={this.state}>{children}</UserContext.Provider>
        );
    }
}
