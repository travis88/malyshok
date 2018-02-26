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
                isCompleted: false
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
        return (
            <div className="import-info">

                <Steps current={this.state.data.step}>
                    <Steps.Step title="загрузка" />
                    <Steps.Step title="обработка" />
                    <Steps.Step title="финализация" />
                </Steps>
                <CircularProgressbar percentage={this.state.data.percent} />
                <h1>Кол-во продуктов: {this.state.data.count}</h1>
                <p>{this.state.data.time}</p>
          </div>
        );
    }
}

ReactDOM.render(
    <ImportInfo url="/admin/import/importprocessed" pollInterval={1000} />,
    document.getElementById('content')
); 