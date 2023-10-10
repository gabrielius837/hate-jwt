const TOKEN_API = 'https://localhost:7188/api/v1/token/issue'
const EXAMPLE_API = 'https://localhost:7083/WeatherForecast'
const tokenPayload = {
    clientId: "backend",
    clientSecret: "I9NHAuarLurqbdRylrSrZQVm+xn119l1Y3gW+7DkG34="
}

const getToken = async () => {
    const resp = await fetch(
        TOKEN_API,
        {
            method: 'POST',
            headers: {
                accept: 'application/json',
                'content-type': 'application/json'
            },
            body: JSON.stringify(tokenPayload)
        }
    )

    return resp.ok ? await resp.json() : null
}

const getExample = async (tokenPayload) => {
    const { token, schema } = tokenPayload
    const resp = await fetch(
        EXAMPLE_API,
        {
            method: 'GET',
            headers: {
                accept: 'text/plain',
                authorization: `${schema} ${token}`
            }
        }
    )

    const msg = await resp.text()

    console.error(resp.status)
    console.error(msg)

    return resp.ok
}

const main = async () => {
    const authResponse = await getToken()
    if (!authResponse) {
        console.error('failed to authenticate')
        return
    }

    console.log('managed to authenticate')

    const exampleStatus = await getExample(authResponse)
    exampleStatus
        ? console.log('managed to get weather')
        : console.error('failed to get weather')
}

main()
    .then(() => process.exit(1))
    .catch(err => {
        console.error(err)
        process.exit(1)
    })