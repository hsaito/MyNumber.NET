pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                echo 'Building..'
                                powershell label: '', script: '''dotnet restore; dotnet build'''
            }
        }

	stage('Test') {
	    steps {
		echo 'Testing..'
		powershell label: '', script: '''dotnet restore; dotnet test -c Release'''
            }
	}
        stage('Deploy') {
            steps {
                echo 'Deploying..'
                powershell label: '', script: '''dotnet restore; dotnet publish -c Release'''
		powershell label: '', script: '''dotnet pack -c Release'''
                archiveArtifacts artifacts: 'MyNumberNET/bin/Release/netstandard2.0/publish/**'
		archiveArtifacts artifacts: 'MyNumberNET/bin/Release/*.nupkg'
		archiveArtifacts artifacts: 'MyNumberNET_CLI/bin/Release/netcoreapp2.1/publish/'
            }
        }

    }
}
