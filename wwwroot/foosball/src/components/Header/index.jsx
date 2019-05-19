import React from 'react';
import { Link, NavLink } from 'react-router-dom';
import { AppBar, Tabs, Tab, Toolbar, Typography } from '@material-ui/core';
import styled from 'styled-components';
import { UserContext } from 'containers/App/userContext';
import Img from 'components/Img';
import Toughlex from 'assets/toughlex.svg';

const CustomNavLink = ({label, link}) => (
    <Link to={link}>
        <Tab label={label} />
    </Link>
);

const CustomTab = ({label, link, ...props}) => (
    <Tab label={label} component={Link} to={link} {...props}/>
);

const StyledLink = styled(CustomNavLink)`
    text-decoration: none;

    text-color: white;
    &:focus, &:hover, &:visited, &:link, &:active {
        text-decoration: none;
    }
`;

const Logo = ({className}) => (
    <Img className={className} src={Toughlex} alt="toughlex" />
);

const StyledLogo = styled(Logo)`
    padding: 0 5px;
    @media (max-width: 599px) {
        max-height: 28px;
    }
    @media (min-width: 600px) {
        max-height: 32px;
    }
`;

const LogoText = ({className}) => (
    <Typography className={className} color="inherit" >Inspired by</Typography>
);

const StyledLogoText = styled(LogoText)`
    white-space:nowrap;
    @media (max-width: 599px) {
        display: none !important;
    }
`;

const LogoWithText = ({className}) => (
    <div className={className}>
        <StyledLogoText />
        <StyledLogo />
    </div>
);

const StyledLogoWithText = styled(LogoWithText)`
    display: flex;
    align-items: center;
`;

const StyledToolbar = styled(Toolbar)`
    min-height: 48px !important;
`

const StyledContainer = styled.div`
    padding: 20px;
    overflow: hidden;
    word-break: break-word;
`;

export class Header extends React.Component{
    state = {
        value: 0,
    };

    handleChange = (event, value) => {
        console.log(event);
        console.log(value);
        this.setState({ value });
    };
    
    render(){
        const { value } = this.state;
        return (
            <>
                <UserContext.Consumer>
                    { ({ authed, name }) => 
                        <AppBar position="static">
                            <StyledToolbar>
                                <StyledLogoWithText />
                                <Tabs value={value} onChange={(e,v) => this.handleChange(e,v)} variant="scrollable">
                                    <CustomTab label="Homepage" link="/" />
                                    <CustomTab label="tournaments" link="/tournaments" />
                                    <CustomTab label="New game" link="/game" />
                                    <CustomTab label="Statistics"link="/statistics" />
                                </Tabs>
                            </StyledToolbar>
                        </AppBar>
                    }
                </UserContext.Consumer>                    
                <StyledContainer>
                    {this.props.children}
                </StyledContainer>
            </>
        )
    }
}