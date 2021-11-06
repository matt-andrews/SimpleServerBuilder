import React from 'react';
import '../index.css';

export default class Login extends React.Component{
    constructor(props){
        super(props);
        this.state = {
            ip: 'ws://' + window.location.hostname + ':9060',
        };
        
        this.handleConnectClick = this.handleConnectClick.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }
    handleConnectClick(){
        this.props.onConnect(this.state.ip);
    }
    handleInputChange(event){
        this.setState({ip: event.target.value});
    }
    render(){
        return(
            <div>
                <h2>Welcome!</h2>
                <br/>
                <div>
                    <p>
                    Server Endpoint:
                    </p>
                    <input type="text" value={this.state.ip} onChange={this.handleInputChange}/>
                    <button onClick={this.handleConnectClick}>Connect</button>
                </div>
            </div>
        );
    };
}