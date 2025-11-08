// Deploy using rg scope
// az deployment group create --name memquranDeploy1 --resource-group rg-memquran-staging-001 --parameters memquran.bicepparam --debug

// Deploy using subscription scope
// az deployment sub create --name memquranDeploy1 --location uksouth --parameters memquran.bicepparam --debug
  
using './memquran.bicep'

@allowed(['dev', 'staging', 'prod'])
@description('Variables defined for this Environment')
param project_name = 'memquran'
param project_suffix  = '001'
param environment_name = 'dev'
param location = 'uksouth'

param key_vault_key_values = [
  {
    name: 'AwsHostSettings--AccessKey'
    value: 'XXX'
  }
  {
    name: 'AwsHostSettings--ProfileName'
    value: ''
  }
  {
    name: 'AwsHostSettings--RegionEndpoint'
    value: 'XXX'
  }
  {
    name: 'AwsHostSettings--SecretKey'
    value: 'XXX'
  }
  {
    name: 'AwsHostSettings--ServiceUrl'
    value: ''
  }
  {
    name: 'ConnectionStrings--Redis'
    value: 'XXX'
  }
  {
    name: 'BetterStackSettings--BearerToken'
    value: 'XXX'
  }
]

@description('Resource Group Name')
param rg_name = toLower('rg-${project_name}-${environment_name}-${project_suffix}')

@description('Key Vault Name')
param vaults_kv_name = toLower('kv-${project_name}-${environment_name}-${project_suffix}')
