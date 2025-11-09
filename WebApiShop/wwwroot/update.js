const h1 = document.querySelector("h1")
const currentUser = sessionStorage.getItem("user")
h1.textContent = `שלום ${JSON.parse(currentUser).firstName} בוא נצלול פנימה`

async function update() {
    const ind = JSON.parse(currentUser).userId;

    const userName = document.querySelector("#userName").value
    const firstName = document.querySelector("#firstName").value
    const lastName = document.querySelector("#lastName").value
    const password = document.querySelector("#password").value

    let updateUserData = {
        firstName,
        lastName,
        userName,
        password
    };

    console.log(updateUserData, ind);
    const response = await fetch(`api/Users/${ind}`,
        {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updateUserData)
        });
    updateUserData = {
        userId:ind,
        firstName,
        lastName,
        userName,
        password
    };
    const userData = await response;
    sessionStorage.setItem("user", JSON.stringify(updateUserData));
    alert("עודכנו הפרטים")

}