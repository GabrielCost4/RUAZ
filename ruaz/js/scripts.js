var quantidade = 0;

var contador = document.getElementById('cart-count');
var botoes = document.querySelectorAll('.adicionarCarrinho');

botoes.forEach(function (botao) {

  botao.addEventListener('click', function () {

    if (botao.classList.contains('ativo')) {

      // remover
      if (quantidade > 0) {
        quantidade--;
        contador.innerText = quantidade;
        alert('Produto removido com sucesso!')
      }

      botao.innerText = 'CARRINHO';
      botao.classList.remove('ativo');

    } else {

      // adicionar
      quantidade++;
      contador.innerText = quantidade;
      alert("Produto adicionado com sucesso!")
      botao.innerText = 'REMOVER';
      botao.classList.add('ativo');

    }

  });

});

// Formulário - Validação Email e Senha
var btnValidar = document.getElementById('btnValidacao')


btnValidar.addEventListener('click', validarEmail)
btnValidar.addEventListener('click', validarSenha)
btnValidar.addEventListener('click', login)

function validarEmail() {
  var email = document.getElementById('inpt-email').value
  var statusEm = document.getElementById('status-email')

  if (email.includes('@') && email.includes('.') && email.length > 5) {
    statusEm.innerHTML = 'Tudo certo! ✅'
    statusEm.style.color = 'green'
    statusEm.style.marginLeft = '20px'
  } else {
    statusEm.innerHTML = 'Algo deu errado! ❌'
    statusEm.style.color = 'red'
    statusEm.style.marginLeft = '20px'
  }
}

function validarSenha() {
  var senha = document.getElementById('inpt-senha').value
  var statusSe = document.getElementById('status-senha')

  var tamanhoCerto = senha.length >= 8
  var temMinuscula = /[a-z]/.test(senha)
  var temMaiuscula = /[A-Z]/.test(senha)
  var temNumero = /\d/.test(senha)
  var temSimbolo = /[^a-zA-Z0-9]/.test(senha)

  if (tamanhoCerto && temMinuscula && temMaiuscula && temNumero && temSimbolo) {
    statusSe.innerHTML = 'Tudo certo! ✅'
    statusSe.style.color = 'green'
    statusSe.style.marginLeft = '20px'
  } else {
    statusSe.innerHTML = 'Requisitos não atingidos! ❌'
    statusSe.style.color = 'red'
    statusSe.style.marginLeft = '20px'
  }
}

// Carregando informações e validando

function login() {
  var senha = document.getElementById('inpt-senha').value
  var email = document.getElementById('inpt-email').value
  var statusInformacoes = document.getElementById('status-informacoes')

  var emailOK = email.includes('@') && email.includes('.') && email.length > 5

  var tamanhoCerto = senha.length >= 8
  var temMinuscula = /[a-z]/.test(senha)
  var temMaiuscula = /[A-Z]/.test(senha)
  var temNumero = /\d/.test(senha)
  var temSimbolo = /[^a-zA-Z0-9]/.test(senha)

  var senhaOK = tamanhoCerto && temMinuscula && temMaiuscula && temNumero && temSimbolo

  if (emailOK && senhaOK) {
    statusInformacoes.innerText = 'Verificando informações...'
    statusInformacoes.style.color = 'orange'
    statusInformacoes.style.webkitTextStrokeColor = '#00000094'
    statusInformacoes.style.webkitTextStrokeWidth = '1px'
    statusInformacoes.style.fontSize = '20px'
    statusInformacoes.style.fontWeight = 'bold'


    setTimeout(() => {
      statusInformacoes.innerText = "Seja bem-vindo! ✅"
      statusInformacoes.style.color = "green"
    }, 2000)
  }
}

// Ver senha

const btn = document.getElementById("verSenha");
const input = document.getElementById("inpt-senha");
const icon = btn.querySelector("i");

btn.addEventListener("click", () => {
  if (input.type === "password") {
    input.type = "text";
    icon.classList.remove("bi-eye-fill");
    icon.classList.add("bi-eye-slash-fill");
  } else {
    input.type = "password";
    icon.classList.remove("bi-eye-slash-fill");
    icon.classList.add("bi-eye-fill");
  }
});


// Roupas

function trocarImagem(img) {
  document.getElementById("img-principal").src = img.src;
}

// Tamanho
function selecionarTamanho(el) {
  document.querySelectorAll(".tamanhos span").forEach(e => e.classList.remove("active"));
  el.classList.add("active");
}

// Cor
document.querySelectorAll(".cor").forEach(cor => {
  cor.addEventListener("click", function () {
    document.querySelectorAll(".cor").forEach(c => c.classList.remove("active"));
    this.classList.add("active");
  });
});


// CARRINHO GLOBAL

function getCarrinho() {
  return JSON.parse(localStorage.getItem("carrinho")) || [];
}

function salvarCarrinho(carrinho) {
  localStorage.setItem("carrinho", JSON.stringify(carrinho));
}

function atualizarContador() {
  const carrinho = getCarrinho();
  const total = carrinho.reduce((soma, item) => soma + item.qtd, 0);

  const el = document.getElementById("cart-count");
  if (el) el.innerText = total;
}

// ADICIONAR PRODUTO

function adicionarCarrinho(produto) {
  let carrinho = getCarrinho();

  const existente = carrinho.find(p =>
    p.nome === produto.nome && p.tamanho === produto.tamanho
  );

  if (existente) {
    existente.qtd++;
  } else {
    produto.qtd = 1;
    carrinho.push(produto);
  }

  salvarCarrinho(carrinho);
  atualizarContador();

  alert("Produto adicionado ao carrinho com sucesso!");
}

function addProduto(nome, preco, imagem) {
  const tamanho = document.querySelector(".tamanhos .active")?.innerText || "Único";

  const produto = {
    nome: nome,
    preco: preco,
    imagem: imagem,
    tamanho: tamanho
  };

  adicionarCarrinho(produto);
}

function renderCarrinho() {
  const carrinho = JSON.parse(localStorage.getItem("carrinho")) || [];
  const container = document.querySelector(".carrinho-produtos");
  const resumo = document.querySelector(".resumo-total strong");

  container.innerHTML = "";

  // SE ESTIVER VAZIO
  if (carrinho.length === 0) {
    container.innerHTML = "<p>Seu carrinho está vazio...</p>";
    resumo.innerText = "R$ 0,00";
    return;
  }

  let total = 0;

  carrinho.forEach((item, index) => {
    total += item.preco * item.qtd;

    container.innerHTML += `
      <div class="item-carrinho">
        <img src="${item.imagem}">
        <div class="item-info">
          <h5>${item.nome}</h5>
          <p>Tamanho: ${item.tamanho}</p>
          <span>R$ ${item.preco.toFixed(2)}</span>
        </div>

        <div class="item-qtd">
          <button onclick="diminuir(${index})">-</button>
          <span>${item.qtd}</span>
          <button onclick="aumentar(${index})">+</button>
        </div>

        <i class="bi bi-trash remover" onclick="remover(${index})"></i>
      </div>
    `;
  });

  resumo.innerText = "R$ " + total.toFixed(2);
}