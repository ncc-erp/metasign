export function SetLocalStorageContract(idContract: number, step: number, isDelete: boolean) {
    if (localStorage.getItem('contracts')) {
        let contracts = JSON.parse(localStorage.getItem('contracts') as string)

        if (contracts.map(contract => contract.contractId).includes(idContract)) {
            if (isDelete) {
                contracts = contracts.filter(contract => contract.contractId !== idContract)
            } else {
                contracts.map(contract => {
                    if (contract.contractId === idContract) {
                        contract.step = step
                    }
                    return contract
                })
            }
        } else {
            contracts.push({ contractId: idContract, step: step })
        }
        let contractsJson = JSON.stringify(contracts)
        localStorage.setItem('contracts', contractsJson)
    }
    else {
        let contracts = [{ contractId: idContract, step: step }]
        let contractsJson = JSON.stringify(contracts)
        localStorage.setItem('contracts', contractsJson)
    }
}
