import * as React from "react";
import {BackendMethods} from "../../renderer";

interface IState {
    status: StatusColor
}

interface IProps {
    title: StatusTitle
}

export enum StatusTitle {
    ServerModule = "Server module connection"
}

export enum StatusColor {
    Red = "red",
    Yellow = "yellow",
    Green = "green"
}

export class CGIConnectionStatus extends React.Component<IProps, IState> {
    constructor(props: IProps) {
        super(props);
        this.state = {
            status: StatusColor.Red
        };
    }

    componentDidMount() {
        const { ipcRenderer } = require('electron');
        ipcRenderer.on('CGIConnection', (event, message) => {
            this.setState({
                status: StatusColor.Yellow
            });
            ipcRenderer.send('call-backend-method', { method: BackendMethods.EstablishCGIConnection, argument: "" });
            ipcRenderer.on('reply-backend-method-' + BackendMethods.EstablishCGIConnection, (event, arg) => {
                if(arg == "Established") {
                    this.setState({
                        status: StatusColor.Green
                    });
                }
            })
        });
    }

    public render(): React.ReactNode {
        return (
            <div title={this.props.title} className={"status " + this.state.status}></div>
        );
    }
}