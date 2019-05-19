import React, { useContext } from 'react';
import { PlayersContext } from 'containers/App/playersContext';
import { UserContext } from 'containers/App/userContext';
import { Login } from './login';

class Homepage extends React.Component{
    redirectToGoogle(){
        var devRedirect = "http://localhost:5000/api/user/googlecallback";
        var prdRedirect = "https://toughbattle20190313122521.azurewebsites.net/api/user/googlecallback";
        if (process.env.NODE_ENV === 'development'){
            window.location = 'https://accounts.google.com/o/oauth2/auth?response_type=code&redirect_uri='+devRedirect+'&scope=openid%20email&https://www.googleapis.com/auth/userinfo.profile&client_id=29993550260-8g7i50ab24jdcn3fjaj4g1jd9lrn2423.apps.googleusercontent.com';
        }
        else{
            window.location = 'https://accounts.google.com/o/oauth2/auth?response_type=code&redirect_uri='+prdRedirect+'&scope=openid%20email&https://www.googleapis.com/auth/userinfo.profile&client_id=29993550260-8g7i50ab24jdcn3fjaj4g1jd9lrn2423.apps.googleusercontent.com';
        }
    }

    render(){
        return (
            <UserContext.Consumer>
            {({ logIn, authed, token }) => (
                <PlayersContext.Consumer>
                    {({ loading, error, players }) =>  (
                        <>
                            <div>Token: {token}</div>
                            <div>Authed: {authed}</div>
                            <div>Homepage</div>
                            <button onClick={this.redirectToGoogle}>Google Login</button>
                            <Login {...this.props} login={logIn} authed={authed} token={this.props.match.params.token} />
                        </>
                    )}
                </PlayersContext.Consumer>                  
            )}
            </UserContext.Consumer>
        )}
}

export default Homepage;