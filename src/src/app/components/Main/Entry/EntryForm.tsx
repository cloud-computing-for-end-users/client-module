import * as React from "react";
import {EntryType} from "./Entry";
import {BackendMethods} from "../../../renderer";
const { ipcRenderer } = require('electron');
    

interface IState { 
  email: string,
  password: string,
  errorMessage: string
}

interface IProps {
  entryType: EntryType
  onLoggedInChange: any
}

export class EntryForm extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleInputChange = this.handleInputChange.bind(this); 
    this.handleSubmit = this.handleSubmit.bind(this); 
    this.state = {
      email: "",
      password: "",
      errorMessage: ""
    }
  }

  componentDidUpdate(prevProps : any) : void {
    if(this.props.entryType !== prevProps.entryType) {
      this.setState({errorMessage: ""});
    }
  }

  componentWillUnmount() {
    ipcRenderer.removeAllListeners('reply-backend-method-' + this.GetBackendMethod());
  }

  private handleSubmit(): void {
    event.preventDefault();

    ipcRenderer.send('call-backend-method', {
      method: this.GetBackendMethod(), 
      argument: {
        Email: this.state.email,
        Password: this.state.password
      }
    });
    ipcRenderer.on('reply-backend-method-' + this.GetBackendMethod(), (event, arg) => {
      if(arg > 0) {
        this.props.onLoggedInChange(true, arg);
      } else {
        switch(this.props.entryType) {
          case EntryType.Login: this.setState({errorMessage: "Either email or password is not correct"}); break;
          case EntryType.CreateAccount: this.setState({errorMessage: "Such email is already registered"}); break;
        }
      }
    });
  }

  private GetBackendMethod() : BackendMethods {
    switch(this.props.entryType) {
      case EntryType.Login: return BackendMethods.Login;
      case EntryType.CreateAccount: return BackendMethods.CreateAccount;
    }
  }

  private handleInputChange(event: any): void {
    // todo probably not the best solution https://github.com/DefinitelyTyped/DefinitelyTyped/issues/26635
    this.setState<never>({
      [event.target.name]: event.target.value
    });
  }

  public render(): React.ReactNode {
    var buttonText, headingText;
    switch(this.props.entryType) {
      case EntryType.Login: 
        buttonText = "Login";
        headingText = "Login";
        break;
      case EntryType.CreateAccount:
        buttonText = "Create account and login";
        headingText = "Create account";
        break;
    }
    
    return ([
        <h1 key="EntryHeading" className="display-4 d-flex justify-content-center">{headingText}</h1>,
        <form key="EntryForm" onSubmit={this.handleSubmit}>
            <div className="form-group">
                <label htmlFor="emailInput">Email address</label>
                <input value={this.state.email} onChange={this.handleInputChange} name="email" type="email" className="form-control" id="emailInput" placeholder="Enter email" />
            </div>
            <div className="form-group">
                <label htmlFor="passwordInput">Password</label>
                <input value={this.state.password} onChange={this.handleInputChange} name="password" type="password" className="form-control" id="passwordInput" placeholder="Password" />
            </div>
            <div className="d-flex flex-column justify-content-center">
                <span className="error-message">{this.state.errorMessage}</span>
                <button type="submit" className="btn btn-primary">{buttonText}</button>
            </div>
        </form>
    ])
  }
}
