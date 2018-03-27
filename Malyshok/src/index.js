import React, { Component } from 'react'
import ReactDOM from 'react-dom'
import CircularProgressbar from 'react-circular-progressbar';
import Steps, { Step } from 'rc-steps';

class ImportInfo extends Component {
    constructor(props) {
        var today = new Date();
        var dd = today.getDate();
        var MM = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        var HH = today.getHours();
        var mm = today.getMinutes();
        var ss = today.getSeconds();

        super(props);
        this.state = {
            data: {
                percent: 0,
                count: 0,
                step: 0,
                time: `${dd}.${MM}.${yyyy} ${HH}:${mm}:${ss}`,
                isCompleted: false,
                steps: [],
                total: '',
                log: []
            }
        };
        this.loadFromServer = this.loadFromServer.bind(this);
    }

    loadFromServer() {
        var xhr = new XMLHttpRequest();
        xhr.open('get', this.props.url, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ data: data });
        }.bind(this);
        xhr.send();
    }

    componentDidMount() {
        this.loadFromServer();
        window.setInterval(this.loadFromServer, this.props.pollInterval)
    }

    render() {
        const steps = this.state.data.steps.map((s, i) => <Steps.Step key={i} title={s} />);
        const logs = this.state.data.log.map((s, i) => <p key={i}><strong>{s}</strong></p>);
        return (
            <div className="import-info">
                <div className="row">
                    <div className="col-md-12">
                        <Steps current={this.state.data.step}>
                            {steps}
                        </Steps>
                    </div>
                    <div className="col-md-6">
                        <CircularProgressbar percentage={this.state.data.percent} />
                    </div>
                    <div className="col-md-6">
                        <p><strong>Затраченное время:</strong> {this.state.data.total}</p>
                        <p><strong>Текущее время:</strong> {this.state.data.time}</p>
                        <div className="alert alert-info">
                            <h4>Процесс:</h4>
                            {logs}
                        </div>
                    </div>
                </div>
          </div>
        );
    }
}

ReactDOM.render(
    <ImportInfo url="/admin/import/importprocessed" pollInterval={100} />,
    document.getElementById('content')
); 