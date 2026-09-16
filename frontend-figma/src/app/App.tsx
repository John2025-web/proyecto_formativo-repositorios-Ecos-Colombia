import { useState, useEffect, useRef } from "react";
import {
  Mail, Lock, Eye, EyeOff, MapPin, ChevronRight,
  X, Navigation, Calendar, Utensils, Landmark, BookOpen,
  Star, Menu, Globe, LogOut, User, Maximize2, Minimize2, Check,
} from "lucide-react";
import { ImageWithFallback } from "@/app/components/figma/ImageWithFallback";
import colombiaLogo from "@/imports/image-removebg-preview.png";
import colombiaMapImg from "@/imports/stylized-simple-outline-map-of-colombia-icon-blue-sketch-map-of-colombia-illustration-vector-removebg-preview.png";
import { login } from "@/services/api";

const C = {
  navy: "#243C8F",
  red: "#EE4266",
  yellow: "#F4D35E",
  bg: "#F7F7F7",
  white: "#FFFFFF",
};

// ─── TRANSLATIONS ─────────────────────────────────────────────────────────────
const TR: Record<string, Record<string, string>> = {
  ES: {
    navDestinos: "Destinos", navExperiencias: "Experiencias", navCultura: "Cultura", navBlog: "Blog", navContacto: "Contacto",
    heroTitle: "Descubre Colombia", heroSub: "Tierra de diversidad infinita — del Caribe al Amazonas, de los Andes al Pacífico.",
    heroBadge: "bienvenido de vuelta", sectionLabel: "Destinos Destacados", sectionTitle: "¿A dónde quieres ir?",
    sectionBtn: "Ver todos los destinos", cardBtn: "Explorar", howToGet: "Cómo llegar", reserveBtn: "Reservar en Airbnb",
    tabHistoria: "Historia", tabCultura: "Cultura", tabEventos: "Eventos", tabSitios: "Sitios turísticos", tabRestaurantes: "Restaurantes",
    mapTitle: "Mapa de Colombia", mapExpand: "Expandir", mapMinimize: "Minimizar", mapGoogle: "Google Maps",
    cerrarSesion: "Cerrar sesión", miPerfil: "Mi perfil", hola: "Hola",
    departments: "Departamentos", municipalities: "Municipios", parks: "Parques naturales", regions: "Regiones únicas",
    readyToExplore: "¿Listo para explorar", findAccom: "Encuentra el alojamiento perfecto para tu aventura",
    orContinue: "o continúa con", noAccount: "¿No tienes cuenta?", register: "Regístrate",
    signIn: "Iniciar sesión", signingIn: "Iniciando sesión...", continueGoogle: "Continuar con Google",
    welcome: "Bienvenido", signInContinue: "Inicia sesión para continuar", forgotPassword: "¿Olvidaste tu contraseña?",
    email: "Correo electrónico", password: "Contraseña", terms: "Términos de servicio", privacy: "Política de privacidad",
    destMarker: "Destino", cityMarker: "Ciudad",
  },
  EN: {
    navDestinos: "Destinations", navExperiencias: "Experiences", navCultura: "Culture", navBlog: "Blog", navContacto: "Contact",
    heroTitle: "Discover Colombia", heroSub: "Land of infinite diversity — from the Caribbean to the Amazon, from the Andes to the Pacific.",
    heroBadge: "welcome back", sectionLabel: "Featured Destinations", sectionTitle: "Where do you want to go?",
    sectionBtn: "View all destinations", cardBtn: "Explore", howToGet: "Get directions", reserveBtn: "Book on Airbnb",
    tabHistoria: "History", tabCultura: "Culture", tabEventos: "Events", tabSitios: "Tourist sites", tabRestaurantes: "Restaurants",
    mapTitle: "Map of Colombia", mapExpand: "Expand", mapMinimize: "Minimize", mapGoogle: "Google Maps",
    cerrarSesion: "Sign out", miPerfil: "My profile", hola: "Hello",
    departments: "Departments", municipalities: "Municipalities", parks: "Natural parks", regions: "Unique regions",
    readyToExplore: "Ready to explore", findAccom: "Find the perfect accommodation for your adventure",
    orContinue: "or continue with", noAccount: "Don't have an account?", register: "Sign up",
    signIn: "Sign in", signingIn: "Signing in...", continueGoogle: "Continue with Google",
    welcome: "Welcome", signInContinue: "Sign in to continue", forgotPassword: "Forgot your password?",
    email: "Email address", password: "Password", terms: "Terms of service", privacy: "Privacy policy",
    destMarker: "Destination", cityMarker: "City",
  },
  PT: {
    navDestinos: "Destinos", navExperiencias: "Experiências", navCultura: "Cultura", navBlog: "Blog", navContacto: "Contato",
    heroTitle: "Descubra a Colômbia", heroSub: "Terra de diversidade infinita — do Caribe ao Amazonas, dos Andes ao Pacífico.",
    heroBadge: "bem-vindo de volta", sectionLabel: "Destinos em Destaque", sectionTitle: "Para onde você quer ir?",
    sectionBtn: "Ver todos os destinos", cardBtn: "Explorar", howToGet: "Como chegar", reserveBtn: "Reservar no Airbnb",
    tabHistoria: "História", tabCultura: "Cultura", tabEventos: "Eventos", tabSitios: "Pontos turísticos", tabRestaurantes: "Restaurantes",
    mapTitle: "Mapa da Colômbia", mapExpand: "Expandir", mapMinimize: "Minimizar", mapGoogle: "Google Maps",
    cerrarSesion: "Sair", miPerfil: "Meu perfil", hola: "Olá",
    departments: "Departamentos", municipalities: "Municípios", parks: "Parques naturais", regions: "Regiões únicas",
    readyToExplore: "Pronto para explorar", findAccom: "Encontre a acomodação perfeita para sua aventura",
    orContinue: "ou continue com", noAccount: "Não tem uma conta?", register: "Cadastre-se",
    signIn: "Entrar", signingIn: "Entrando...", continueGoogle: "Continuar com Google",
    welcome: "Bem-vindo", signInContinue: "Entre para continuar", forgotPassword: "Esqueceu sua senha?",
    email: "Endereço de e-mail", password: "Senha", terms: "Termos de serviço", privacy: "Política de privacidade",
    destMarker: "Destino", cityMarker: "Cidade",
  },
  FR: {
    navDestinos: "Destinations", navExperiencias: "Expériences", navCultura: "Culture", navBlog: "Blog", navContacto: "Contact",
    heroTitle: "Découvrez la Colombie", heroSub: "Terre de diversité infinie — des Caraïbes à l'Amazonie, des Andes au Pacifique.",
    heroBadge: "bienvenue de retour", sectionLabel: "Destinations phares", sectionTitle: "Où voulez-vous aller?",
    sectionBtn: "Voir toutes les destinations", cardBtn: "Explorer", howToGet: "Comment y aller", reserveBtn: "Réserver sur Airbnb",
    tabHistoria: "Histoire", tabCultura: "Culture", tabEventos: "Événements", tabSitios: "Sites touristiques", tabRestaurantes: "Restaurants",
    mapTitle: "Carte de la Colombie", mapExpand: "Agrandir", mapMinimize: "Réduire", mapGoogle: "Google Maps",
    cerrarSesion: "Se déconnecter", miPerfil: "Mon profil", hola: "Bonjour",
    departments: "Départements", municipalities: "Communes", parks: "Parcs naturels", regions: "Régions uniques",
    readyToExplore: "Prêt à explorer", findAccom: "Trouvez l'hébergement parfait pour votre aventure",
    orContinue: "ou continuez avec", noAccount: "Pas de compte?", register: "S'inscrire",
    signIn: "Se connecter", signingIn: "Connexion...", continueGoogle: "Continuer avec Google",
    welcome: "Bienvenue", signInContinue: "Connectez-vous pour continuer", forgotPassword: "Mot de passe oublié?",
    email: "Adresse e-mail", password: "Mot de passe", terms: "Conditions d'utilisation", privacy: "Politique de confidentialité",
    destMarker: "Destination", cityMarker: "Ville",
  },
  DE: {
    navDestinos: "Reiseziele", navExperiencias: "Erlebnisse", navCultura: "Kultur", navBlog: "Blog", navContacto: "Kontakt",
    heroTitle: "Entdecke Kolumbien", heroSub: "Land der unendlichen Vielfalt — von der Karibik bis zum Amazonas, von den Anden bis zum Pazifik.",
    heroBadge: "willkommen zurück", sectionLabel: "Beliebte Reiseziele", sectionTitle: "Wohin möchtest du reisen?",
    sectionBtn: "Alle Reiseziele anzeigen", cardBtn: "Erkunden", howToGet: "Anfahrt", reserveBtn: "Bei Airbnb buchen",
    tabHistoria: "Geschichte", tabCultura: "Kultur", tabEventos: "Veranstaltungen", tabSitios: "Sehenswürdigkeiten", tabRestaurantes: "Restaurants",
    mapTitle: "Karte Kolumbiens", mapExpand: "Vergrößern", mapMinimize: "Minimieren", mapGoogle: "Google Maps",
    cerrarSesion: "Abmelden", miPerfil: "Mein Profil", hola: "Hallo",
    departments: "Departments", municipalities: "Gemeinden", parks: "Naturparks", regions: "Einzigartige Regionen",
    readyToExplore: "Bereit für", findAccom: "Finde die perfekte Unterkunft für dein Abenteuer",
    orContinue: "oder weiter mit", noAccount: "Noch kein Konto?", register: "Registrieren",
    signIn: "Anmelden", signingIn: "Anmeldung...", continueGoogle: "Mit Google fortfahren",
    welcome: "Willkommen", signInContinue: "Melde dich an, um fortzufahren", forgotPassword: "Passwort vergessen?",
    email: "E-Mail-Adresse", password: "Passwort", terms: "Nutzungsbedingungen", privacy: "Datenschutzrichtlinie",
    destMarker: "Ziel", cityMarker: "Stadt",
  },
};

const LANGUAGES = [
  { code: "ES", label: "Español", flag: "🇨🇴" },
  { code: "EN", label: "English", flag: "🇺🇸" },
  { code: "PT", label: "Português", flag: "🇧🇷" },
  { code: "FR", label: "Français", flag: "🇫🇷" },
  { code: "DE", label: "Deutsch", flag: "🇩🇪" },
];

// ─── MAP MARKERS — percentage positions over the real Colombia image ──────────
const ALL_MARKERS = [
  // Caribbean coast
  { name: "Cartagena",     px: 40,  py: 23,  dest: true  },
  { name: "Barranquilla",  px: 48,  py: 18,  dest: true  },
  { name: "Santa Marta",   px: 54,  py: 19,  dest: true  },
  { name: "Riohacha",      px: 61,  py: 12,   dest: false },
  { name: "Valledupar",    px: 62,  py: 21,  dest: true  },
  { name: "Mompox",        px: 48,  py: 28,  dest: true  },
  { name: "Montería",      px: 31,  py: 27,  dest: false },
  { name: "Palomino",      px: 56,  py: 17,  dest: false },
  { name: "Cabo de la Vela", px: 69, py: 10,  dest: true  },
  // Insular (San Andrés - in Caribbean sea, shown at top-left)
  { name: "San Andrés",    px: 8,   py: 17,  dest: true  },
  // Pacific coast
  { name: "Quibdó",        px: 22,  py: 43,  dest: true  },
  { name: "Nuquí",         px: 14,  py: 39,  dest: false },
  { name: "Buenaventura",  px: 18,  py: 56,  dest: false },
  // Andes / Centro
  { name: "Medellín",      px: 37,  py: 42,  dest: true  },
  { name: "Guatapé",       px: 40,  py: 39,  dest: true  },
  { name: "Manizales",     px: 35,  py: 48,  dest: true  },
  { name: "Pereira",       px: 34,  py: 51,  dest: true  },
  { name: "Salento",       px: 35,  py: 53,  dest: true  },
  { name: "Bogotá",        px: 50,  py: 52,  dest: true  },
  { name: "Villa de Leyva",px: 51,  py: 45,  dest: true  },
  { name: "Bucaramanga",   px: 58,  py: 37,  dest: true  },
  { name: "Cali",          px: 29,  py: 59,  dest: true  },
  { name: "Popayán",       px: 27,  py: 65,  dest: true  },
  { name: "Tatacoa",       px: 43,  py: 62,  dest: true  },
  { name: "San Agustín",   px: 31,  py: 70,  dest: true  },
  { name: "Pasto",         px: 24,  py: 76,  dest: true  },
  // Orinoquía / Este
  { name: "Arauca",        px: 74,  py: 36,  dest: true  },
  { name: "Villavicencio", px: 56,  py: 59,  dest: false },
  { name: "Caño Cristales",px: 53,  py: 67,  dest: true  },
  // Amazonía / Sur
  { name: "Florencia",     px: 50,  py: 74,  dest: false },
  { name: "Puerto Nariño", px: 67,  py: 87,  dest: true  },
  { name: "Leticia",       px: 68,  py: 91,  dest: true  },
];

// ─── NAV SECTIONS ─────────────────────────────────────────────────────────────
const NAV_ITEMS_MAP: Record<string, { title: string; desc: string; items: { label: string; destId?: number }[] }> = {
  Destinos: {
    title: "Destinos de Colombia",
    desc: "Explora destinos increíbles del Caribe, Andes, Pacífico, Orinoquía y Amazonía.",
    items: [
      { label: "Cartagena de Indias", destId: 1 },
      { label: "Bogotá D.C.", destId: 2 },
      { label: "Medellín", destId: 3 },
      { label: "Santa Marta", destId: 4 },
      { label: "San Andrés", destId: 8 },
      { label: "Guatapé", destId: 10 },
      { label: "Salento (Eje Cafetero)", destId: 11 },
      { label: "Popayán", destId: 12 },
      { label: "Caño Cristales", destId: 16 },
      { label: "Cabo de la Vela", destId: 15 },
      { label: "Cali", destId: 18 },
      { label: "Barranquilla", destId: 19 },
      { label: "Leticia (Amazonas)", destId: 7 },
      { label: "San Agustín", destId: 13 },
      { label: "Mompox", destId: 14 },
      { label: "Ver todos →", destId: undefined },
    ],
  },
  Experiencias: {
    title: "Experiencias únicas",
    desc: "Vive Colombia de manera auténtica.",
    items: [
      { label: "Senderismo y naturaleza" },
      { label: "Gastronomía local" },
      { label: "Turismo de café" },
      { label: "Avistamiento de aves" },
      { label: "Surf y playas" },
      { label: "Turismo indígena" },
      { label: "Buceo y snorkel" },
      { label: "Ciclismo urbano" },
    ],
  },
  Cultura: {
    title: "Cultura colombiana",
    desc: "Descubre la rica herencia cultural de Colombia.",
    items: [
      { label: "Festivales y carnavales" },
      { label: "Música vallenata" },
      { label: "Arte y museos" },
      { label: "Gastronomía" },
      { label: "Artesanías" },
      { label: "Arquitectura colonial" },
      { label: "Danzas tradicionales" },
      { label: "Literatura colombiana" },
    ],
  },
  Blog: {
    title: "Blog de viajes",
    desc: "Guías, consejos e historias de Colombia.",
    items: [
      { label: "Guías de viaje" },
      { label: "Itinerarios recomendados" },
      { label: "Tips de seguridad" },
      { label: "Viaje con presupuesto" },
      { label: "Mejores épocas para viajar" },
      { label: "Fotografía de viaje" },
      { label: "Historias reales" },
      { label: "Novedades turísticas" },
    ],
  },
  Contacto: {
    title: "Contáctanos",
    desc: "Estamos aquí para ayudarte a planear el viaje de tu vida.",
    items: [
      { label: "Soporte 24/7" },
      { label: "WhatsApp" },
      { label: "Correo electrónico" },
      { label: "Oficinas en Colombia" },
      { label: "Partners turísticos" },
      { label: "Prensa y medios" },
      { label: "Trabaja con nosotros" },
      { label: "Preguntas frecuentes" },
    ],
  },
};

// ─── DESTINATIONS DATA (25 destinos) ─────────────────────────────────────────
const destinations = [
  // ── 1. CARTAGENA ──────────────────────────────────────────────────────────
  {
    id: 1, name: "Cartagena", region: "Costa Caribe", tagline: "La Ciudad Amurallada",
    description: "Joya colonial del Caribe colombiano, con murallas centenarias, playas cristalinas y una vibrante vida nocturna.",
    image: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=1200&h=500&fit=crop&auto=format",
    rating: 4.9, weather: "28°C · Soleado", mapsQuery: "Cartagena+de+Indias+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Cartagena--Colombia/homes",
    historia: "Fundada en 1533 por Pedro de Heredia, Cartagena de Indias fue el principal puerto del Imperio español en América. Sus imponentes murallas, construidas a lo largo de cuatro siglos, la convirtieron en una de las plazas fuertes más inexpugnables del Nuevo Mundo. El proceso de construcción de sus fortificaciones tomó más de 200 años y utilizó mano de obra esclava africana, lo que dejó una profunda huella cultural en la ciudad.\n\nEn 1984, la UNESCO declaró su centro histórico Patrimonio de la Humanidad, reconociendo la excepcional conservación de su arquitectura colonial. Durante la época colonial, Cartagena fue el principal punto de entrada de esclavos africanos a América del Sur y uno de los puertos de salida del oro y las riquezas del continente hacia España.\n\nEn 1811, Cartagena proclamó su independencia de España convirtiéndose en una de las primeras ciudades hispanoamericanas en hacerlo, un hecho histórico que se celebra cada 11 de noviembre. Hoy alberga más de un millón de habitantes y recibe más de dos millones de turistas al año, siendo el destino turístico más visitado de Colombia.",
    cultura: "La cultura cartagenera es una fusión vibrante de herencias españolas, africanas e indígenas que se refleja en cada rincón de la ciudad. La música es el alma de Cartagena: el vallenato, la cumbia y el porro llenan sus calles durante el día, mientras que los ritmos afrocolombianos del Palenque de San Basilio —primer pueblo libre de América, declarado Patrimonio Inmaterial por la UNESCO— resuenan en sus fiestas.\n\nSu gastronomía es igualmente rica: el arroz con coco, el ceviche cartagenero, los camarones, el bollo de mazorca y la cocada son expresiones de su identidad costeña. Las calles del centro histórico están llenas de vendedoras de frutas con trajes de colores, música en vivo y el olor inconfundible del mar Caribe mezclado con las flores de los balcones coloniales.\n\nEl Hay Festival Cartagena convoca anualmente a intelectuales, escritores y artistas de todo el mundo, consolidando a la ciudad como capital cultural del Caribe colombiano. Las discotecas del Getsemaní y los bares del centro amurallado son el epicentro de una vida nocturna internacionalmente reconocida.",
    eventos: [
      { nombre: "Festival Internacional de Cine", fecha: "Marzo 2025", icono: "🎬" },
      { nombre: "Hay Festival Cartagena", fecha: "Enero 2025", icono: "📚" },
      { nombre: "Fiestas de Independencia (11 de noviembre)", fecha: "Noviembre 2025", icono: "🎊" },
    ],
    sitios: [
      { nombre: "Castillo San Felipe de Barajas", foto: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=300&h=200&fit=crop&auto=format", mapsQuery: "Castillo+San+Felipe+Cartagena" },
      { nombre: "Ciudad Amurallada", foto: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=300&h=200&fit=crop&auto=format", mapsQuery: "Ciudad+Amurallada+Cartagena" },
      { nombre: "Islas del Rosario", foto: "https://images.unsplash.com/photo-1618426018046-77564bc4b0eb?w=300&h=200&fit=crop&auto=format", mapsQuery: "Islas+del+Rosario+Colombia" },
    ],
    restaurantes: [
      { nombre: "La Vitrola", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina caribeña · Jazz en vivo", mapsQuery: "La+Vitrola+Cartagena" },
      { nombre: "El Gobernador", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Mariscos frescos", mapsQuery: "El+Gobernador+Cartagena" },
      { nombre: "Carmen Restaurant", foto: "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=300&h=200&fit=crop&auto=format", tipo: "Fusión colombiana", mapsQuery: "Carmen+Restaurant+Cartagena" },
    ],
  },
  // ── 2. BOGOTÁ ─────────────────────────────────────────────────────────────
  {
    id: 2, name: "Bogotá", region: "Sabana de Bogotá", tagline: "La Capital Cultural",
    description: "Capital cultural y gastronómica de Colombia. Museos de clase mundial, barrios bohemios y una escena artística sin igual.",
    image: "https://images.unsplash.com/photo-1759375352933-c96abe2e4be7?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1759375368039-7df9ca6e7307?w=1200&h=500&fit=crop&auto=format",
    rating: 4.7, weather: "14°C · Nublado", mapsQuery: "Bogotá+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Bogot%C3%A1--Colombia/homes",
    historia: "Fundada el 6 de agosto de 1538 por Gonzalo Jiménez de Quesada, Santa Fe de Bogotá se estableció en el altiplano cundiboyacense, sobre los territorios del pueblo muisca, a 2.600 metros sobre el nivel del mar. Desde la época colonial fue la capital del Virreinato de la Nueva Granada y el centro del poder político del norte de América del Sur.\n\nEl 20 de julio de 1810, el Grito de Independencia resonó desde sus calles, dando inicio a la lucha emancipadora que culminaría con la liberación de Colombia en 1819. El Bogotazo de 1948, el asesinato del caudillo Jorge Eliécer Gaitán, marcó el inicio de décadas de violencia política y transformó profundamente el paisaje social y urbano de la ciudad.\n\nA lo largo del siglo XX, Bogotá se transformó en una megalópolis que hoy supera los 8 millones de habitantes, siendo la quinta ciudad más grande de América Latina. Su Transmilenio, inaugurado en 2000, es uno de los sistemas de transporte masivo más grandes del mundo. En 2012 fue reconocida como la Ciudad Iberoamericana del Libro, y sus museos —el del Oro, el Botero, el Nacional— la posicionan como una de las capitales culturales más importantes del continente.",
    cultura: "Bogotá es reconocida mundialmente como Ciudad Creativa de la Gastronomía por la UNESCO, un título que refleja la extraordinaria diversidad culinaria de sus más de 17.000 restaurantes. Sus barrios concentran universos culturales distintos: La Candelaria guarda la memoria colonial con sus casas de fachadas coloridas y museos; Usaquén sorprende con su mercado de pulgas dominical y su ambiente europeo; Chapinero es el corazón bohemio y LGBTQ+ de la ciudad; y el Parque de la 93 centraliza la alta gastronomía y el diseño de moda.\n\nBogotá organiza el festival de jazz más importante de Suramérica, el Festival Iberoamericano de Teatro —el segundo más grande del mundo— y el Rock al Parque, uno de los festivales de rock gratuitos más masivos del planeta. El Museo del Oro custodia la mayor colección de orfebrería precolombina del mundo, con más de 55.000 piezas de oro.\n\nSu sistema de ciclovías dominicales, con más de 121 km de vías habilitadas, es modelo para ciudades de todo el mundo. El graffiti art de sus barrios bohemios es reconocido internacionalmente como una de las expresiones urbanas más vibrantes de América Latina.",
    eventos: [
      { nombre: "Rock al Parque", fecha: "Julio 2025", icono: "🎸" },
      { nombre: "Feria Internacional del Libro", fecha: "Abril 2025", icono: "📖" },
      { nombre: "Festival Iberoamericano de Teatro", fecha: "Marzo 2026", icono: "🎭" },
    ],
    sitios: [
      { nombre: "Museo del Oro (Banco de la República)", foto: "https://images.unsplash.com/photo-1759375352933-c96abe2e4be7?w=300&h=200&fit=crop&auto=format", mapsQuery: "Museo+del+Oro+Bogota" },
      { nombre: "Cerro Monserrate", foto: "https://images.unsplash.com/photo-1759375368039-7df9ca6e7307?w=300&h=200&fit=crop&auto=format", mapsQuery: "Cerro+Monserrate+Bogota" },
      { nombre: "Barrio La Candelaria", foto: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=300&h=200&fit=crop&auto=format", mapsQuery: "La+Candelaria+Bogota" },
    ],
    restaurantes: [
      { nombre: "Criterion", foto: "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=300&h=200&fit=crop&auto=format", tipo: "Alta cocina colombiana", mapsQuery: "Criterion+Restaurante+Bogota" },
      { nombre: "Leo Cocina y Cava", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina biodiversa", mapsQuery: "Leo+Cocina+Cava+Bogota" },
      { nombre: "Andrés Carne de Res", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Parrilla colombiana festiva", mapsQuery: "Andres+Carne+de+Res+Bogota" },
    ],
  },
  // ── 3. MEDELLÍN ───────────────────────────────────────────────────────────
  {
    id: 3, name: "Medellín", region: "Valle de Aburrá", tagline: "La Ciudad de la Eterna Primavera",
    description: "Transformación urbana ejemplar del mundo. Innovación, flores, metrocable y la cálida cultura paisa.",
    image: "https://images.unsplash.com/photo-1515366974328-f1181eb25189?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1653932610416-399b3deb52d2?w=1200&h=500&fit=crop&auto=format",
    rating: 4.8, weather: "22°C · Parcialmente nublado", mapsQuery: "Medellín+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Medell%C3%ADn--Colombia/homes",
    historia: "Medellín fue fundada oficialmente en 1616, aunque los asentamientos españoles en el Valle de Aburrá comenzaron décadas antes. Durante el siglo XIX se convirtió en el motor industrial de Colombia gracias a su pujante industria textil —las famosas 'fábricas'— que atrajeron migración masiva del campo a la ciudad. El ferrocarril de Antioquia, terminado en 1929, la conectó con el río Magdalena y la convirtió en un nodo comercial estratégico.\n\nSin embargo, las décadas de 1980 y 1990 la sumieron en una violencia extrema vinculada al narcotráfico de Pablo Escobar, que llegó a hacerla la ciudad más peligrosa del mundo con más de 6.000 homicidios en un solo año. A partir de los 2000, emprendió una transformación urbana y social sin precedentes: el Metrocable conectó las comunas más vulnerables con el centro, el urbanismo social mejoró la calidad de vida en los barrios periféricos, y la inversión en educación y cultura cambió el tejido social.\n\nEn 2013, el Urban Land Institute la eligió la ciudad más innovadora del mundo, un reconocimiento que consolidó su nuevo rol como modelo de regeneración urbana para el planeta.",
    cultura: "La cultura paisa es el alma de Medellín: hospitalidad genuina, orgullo regional, espíritu emprendedor y una identidad forjada entre montañas. La Feria de las Flores, celebrada en agosto, es la celebración más emblemática: los silleteros —familias campesinas de Santa Elena— bajan cargando monumentales arreglos florales que pueden pesar más de 100 kilos, en un desfile que reúne a más de 500.000 espectadores.\n\nEl Museo de Antioquia alberga la mayor colección de obras de Fernando Botero, el artista colombiano más reconocido a nivel mundial, quien donó más de 100 esculturas y pinturas a su ciudad natal. El barrio El Poblado concentra la vida nocturna, la gastronomía de vanguardia y el turismo internacional. La escena musical de Medellín va del tango (Carlos Gardel murió aquí en 1935) al reggaetón y el urbano contemporáneo que domina las listas globales.\n\nEl sistema de metro —único en Colombia— junto con el Metrocable y los escaladores eléctricos de la comuna 13, son íconos de innovación social que se estudian en universidades de arquitectura y urbanismo de todo el mundo.",
    eventos: [
      { nombre: "Feria de las Flores", fecha: "Agosto 2025", icono: "🌸" },
      { nombre: "Colombia Moda", fecha: "Julio 2025", icono: "👗" },
      { nombre: "Festival Internacional de Poesía", fecha: "Julio 2025", icono: "✍️" },
    ],
    sitios: [
      { nombre: "Parque Explora", foto: "https://images.unsplash.com/photo-1515366974328-f1181eb25189?w=300&h=200&fit=crop&auto=format", mapsQuery: "Parque+Explora+Medellin" },
      { nombre: "Metrocable y Comunas", foto: "https://images.unsplash.com/photo-1653932610416-399b3deb52d2?w=300&h=200&fit=crop&auto=format", mapsQuery: "Metrocable+Medellin" },
      { nombre: "El Poblado y Parque Lleras", foto: "https://images.unsplash.com/photo-1778188985186-25a9d5f7d2b8?w=300&h=200&fit=crop&auto=format", mapsQuery: "El+Poblado+Medellin" },
    ],
    restaurantes: [
      { nombre: "El Cielo", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Gastronomía molecular colombiana", mapsQuery: "El+Cielo+Restaurante+Medellin" },
      { nombre: "Hacienda", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Cocina paisa tradicional", mapsQuery: "Hacienda+Restaurante+Medellin" },
      { nombre: "Pergamino Café", foto: "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=300&h=200&fit=crop&auto=format", tipo: "Café de especialidad", mapsQuery: "Pergamino+Cafe+Medellin" },
    ],
  },
  // ── 4. SANTA MARTA ────────────────────────────────────────────────────────
  {
    id: 4, name: "Santa Marta", region: "Magdalena", tagline: "La Ciudad más Antigua de Colombia",
    description: "Primera ciudad fundada en América del Sur. Puertas al Parque Tayrona y la Sierra Nevada del mar Caribe.",
    image: "https://images.unsplash.com/photo-1618426018046-77564bc4b0eb?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=1200&h=500&fit=crop&auto=format",
    rating: 4.6, weather: "30°C · Soleado", mapsQuery: "Santa+Marta+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Santa-Marta--Colombia/homes",
    historia: "Santa Marta fue fundada el 29 de julio de 1525 por Rodrigo de Bastidas, convirtiéndose en la primera ciudad fundada por los españoles en el territorio continental de América del Sur. Su bahía, una de las más hermosas del Caribe, fue el punto de entrada desde el cual los conquistadores se adentraron al interior del continente en busca del mítico Eldorado y las tierras de los Muiscas.\n\nLa ciudad vivió sus horas más dolorosas el 17 de diciembre de 1830, cuando Simón Bolívar, El Libertador, exhaló su último aliento en la Quinta de San Pedro Alejandrino, donde hoy se conserva el memorial en su honor. Durante los siglos XIX y XX, la región fue escenario del auge bananero de la United Fruit Company, inmortalizando a Santa Marta en las páginas de 'Cien Años de Soledad' de Gabriel García Márquez.\n\nHoy, Santa Marta es la cuarta ciudad más grande de la Costa Caribe colombiana y el principal acceso a ecosistemas únicos como la Sierra Nevada de Santa Marta, el pico costero más alto del mundo a 5.775 m.s.n.m., y el Parque Nacional Natural Tayrona, uno de los parques más visitados de Colombia.",
    cultura: "La identidad cultural de Santa Marta está profundamente marcada por la presencia de cuatro pueblos indígenas que habitan la Sierra Nevada: los Koguis, guardianes de la memoria de los Tayrona; los Arhuacos, expertos en el tejido de la mochila; los Wiwas, conocidos por sus conocimientos medicinales; y los Kankuamos. Estos pueblos consideran la Sierra Nevada el corazón del mundo y realizan pagamentos rituales para mantener el equilibrio del universo.\n\nLa Ciudad Perdida (Teyuna), construida por los Tayrona entre los siglos VIII y XIV y redescubierta en 1972, es uno de los sitios arqueológicos más impresionantes de América del Sur. El trekking de 4 días para llegar es considerado una de las rutas de aventura más icónicas del continente.\n\nLa gastronomía samaria combina tradiciones indígenas, africanas y españolas: el sancocho de carne, el arroz con mariscos, el friche y los pescados del Caribe son platos imprescindibles. La bahía de Santa Marta alberga una diversidad marina extraordinaria con arrecifes de coral, peces tropicales y tortugas marinas.",
    eventos: [
      { nombre: "Festival del Mar", fecha: "Julio 2025", icono: "🌊" },
      { nombre: "Carnaval del Mar", fecha: "Febrero 2025", icono: "🎉" },
      { nombre: "Festival Vallenato Caribe", fecha: "Octubre 2025", icono: "🪗" },
    ],
    sitios: [
      { nombre: "Parque Nacional Natural Tayrona", foto: "https://images.unsplash.com/photo-1618426018046-77564bc4b0eb?w=300&h=200&fit=crop&auto=format", mapsQuery: "Parque+Nacional+Tayrona" },
      { nombre: "Ciudad Perdida (Teyuna)", foto: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=300&h=200&fit=crop&auto=format", mapsQuery: "Ciudad+Perdida+Sierra+Nevada" },
      { nombre: "Quinta de San Pedro Alejandrino", foto: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=300&h=200&fit=crop&auto=format", mapsQuery: "Quinta+San+Pedro+Alejandrino+Santa+Marta" },
    ],
    restaurantes: [
      { nombre: "La Canoa", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Mariscos frescos del Caribe", mapsQuery: "La+Canoa+Restaurante+Santa+Marta" },
      { nombre: "Ouzo", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Fusión mediterránea-caribeña", mapsQuery: "Ouzo+Restaurante+Santa+Marta" },
    ],
  },
  // ── 5. ARAUCA ─────────────────────────────────────────────────────────────
  {
    id: 5, name: "Arauca", region: "Orinoquía", tagline: "Corazón de los Llanos",
    description: "En la frontera con Venezuela, cuna de la cultura llanera, el joropo y los horizontes infinitos de la Orinoquía colombiana.",
    image: "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1559827291-72ee739d0d9a?w=1200&h=500&fit=crop&auto=format",
    rating: 4.4, weather: "32°C · Cálido", mapsQuery: "Arauca+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Arauca--Colombia/homes",
    historia: "Arauca fue fundada en 1780 a orillas del río Arauca, límite natural con Venezuela, en plena sabana llanera. Su nombre proviene del vocablo indígena 'aruco' que designaba una especie de árbol nativo de la región. Durante el siglo XIX fue escenario de batallas de la Guerra de Independencia: por aquí cruzó el Ejército Patriota en la histórica Campaña Libertadora de 1819, cuando Bolívar y sus tropas cruzaron los Llanos inundados y los Andes hacia la batalla de Boyacá.\n\nLa explotación del caucho, el ganado y más tarde el petróleo —con el descubrimiento del campo Caño Limón en 1983, el mayor de Colombia en ese momento— definieron la economía araucana del siglo XX. El petróleo trajo prosperidad pero también tensiones sociales y conflictos armados que marcaron profundamente a la región durante décadas.\n\nHoy Arauca se proyecta como destino de turismo ecológico y cultural gracias a su posición privilegiada en los Llanos Orientales, con acceso a ecosistemas únicos de sabana inundable, caños y morichales que albergan fauna extraordinaria: caimanes, anacondas, chigüiros, garzas y cientos de especies de aves.",
    cultura: "La cultura llanera es el corazón de Arauca. El hato, la sabana y el río forman el escenario de una vida que gira alrededor del ganado, el caballo y el joropo —música declarada Patrimonio Cultural Inmaterial de la Humanidad que comparte con Venezuela. El coleo, deporte tradicional en el que el jinete tumba un toro tomándolo de la cola, tiene en Arauca uno de sus epicentros más importantes.\n\nLas ferias y fiestas locales celebran la vida llanera con reinados, corralejas y competencias ecuestres. La gastronomía araucana es un reflejo de la sabana: la carne a la llanera (ternera a la vareta asada al fuego lento), el mamona, la hayaca llanera y el tungito son platos que se comparten en comunidad.\n\nEl río Arauca es el epicentro de la vida social, y su malecón concentra restaurantes, artesanías y el intercambio cotidiano entre colombianos y venezolanos. Los morichales —palmerales de moriche en terrenos inundables— son ecosistemas únicos de la Orinoquía que albergan una biodiversidad extraordinaria y representan espacios sagrados para las comunidades indígenas Sikuani y Betoye de la región.",
    eventos: [
      { nombre: "Festival Internacional del Joropo", fecha: "Enero 2025", icono: "🎵" },
      { nombre: "Fiestas de la Frontera", fecha: "Julio 2025", icono: "🤝" },
      { nombre: "Expo Arauca Ganadera", fecha: "Noviembre 2025", icono: "🐄" },
    ],
    sitios: [
      { nombre: "Malecón del Río Arauca", foto: "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?w=300&h=200&fit=crop&auto=format", mapsQuery: "Malecon+Arauca+Colombia" },
      { nombre: "Parque Los Sueños", foto: "https://images.unsplash.com/photo-1559827291-72ee739d0d9a?w=300&h=200&fit=crop&auto=format", mapsQuery: "Parque+Los+Suenos+Arauca" },
      { nombre: "Caños y Morichales del Llano", foto: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Morichales+Arauca+Colombia" },
    ],
    restaurantes: [
      { nombre: "La Terraza Llanera", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Cocina llanera tradicional", mapsQuery: "La+Terraza+Llanera+Arauca" },
      { nombre: "El Palenque", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Carnes y parrilla llanera", mapsQuery: "El+Palenque+Arauca+Colombia" },
    ],
  },
  // ── 6. CHOCÓ (QUIBDÓ) ────────────────────────────────────────────────────
  {
    id: 6, name: "Chocó (Quibdó)", region: "Pacífico", tagline: "El Pulmón Verde de Colombia",
    description: "El departamento más biodiverso del mundo, con selvas vírgenes, playas del Pacífico de ensueño y la rica cultura afrocolombiana.",
    image: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1545558014-8692077e9b5c?w=1200&h=500&fit=crop&auto=format",
    rating: 4.5, weather: "28°C · Lluvioso", mapsQuery: "Quibdó+Chocó+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Quibd%C3%B3--Colombia/homes",
    historia: "Quibdó, capital del departamento de Chocó, fue fundada el 2 de febrero de 1654 por el misionero jesuita fray Matías Abad. El Chocó es el único departamento de Colombia —y uno de los pocos territorios del mundo— con costas en dos océanos: el Pacífico al oeste y el Atlántico (mar Caribe) a través del Golfo de Urabá al norte.\n\nDurante la Colonia, el Chocó fue explotado intensamente por sus inmensas riquezas minerales: oro y platino extraídos por miles de africanos esclavizados, cuya descendencia formó la mayoría de la población actual. En el siglo XIX, el naturalista alemán Alexander von Humboldt describió la región como una de las más ricas en biodiversidad del planeta.\n\nHoy, el Chocó registra el nivel de pluviosidad más alto de Colombia y uno de los más altos del mundo —hasta 13.000 mm anuales en algunas zonas—, lo que sustenta ecosistemas de una complejidad biológica sin igual: más de 8.000 especies de plantas vasculares, 600 especies de aves y 100 especies de mamíferos, muchas de ellas endémicas del planeta.",
    cultura: "La cultura afrocolombiana del Chocó es una de las más ricas y originales de América Latina, forjada en siglos de resistencia, sincretismo y creatividad. La chirimía chocoana —conjunto de flautas traversas, clarinetes, bombos, redoblantes y platillos— es la expresión musical más característica de la región, con raíces en la música de banda española transformada por el ritmo africano.\n\nEl Festival de San Francisco de Asís (San Pacho), celebrado cada octubre en Quibdó durante 11 días de colorido desbordante, fue inscrito en la Lista del Patrimonio Cultural Inmaterial de la Humanidad por la UNESCO en 2012. Además de la comunidad afrodescendiente, el Chocó es hogar de los pueblos indígenas Emberá, Wounaan, Tule (Kuna) y Zenú, que mantienen vivos sus rituales, artesanías y saberes ancestrales.\n\nLas playas del Pacífico chocoano —Nuquí, Bahía Solano, El Valle— son el destino predilecto de las ballenas jorobadas entre julio y noviembre, cuando miles de cetáceos llegan al Pacífico colombiano para reproducirse. La gastronomía chocoana destaca por el uso de frutas tropicales, el chontaduro, el borojó y los mariscos del Pacífico.",
    eventos: [
      { nombre: "Festival de San Pacho (UNESCO)", fecha: "Septiembre-Octubre 2025", icono: "🎊" },
      { nombre: "Avistamiento de Ballenas Jorobadas", fecha: "Julio-Noviembre 2025", icono: "🐋" },
      { nombre: "Semana de la Afrocolombianidad", fecha: "Mayo 2025", icono: "✊" },
    ],
    sitios: [
      { nombre: "Bahía Solano y Playa El Valle", foto: "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=300&h=200&fit=crop&auto=format", mapsQuery: "Bahia+Solano+Chocó+Colombia" },
      { nombre: "Parque Nacional Natural Los Katíos", foto: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Parque+Nacional+Los+Katios+Colombia" },
      { nombre: "Nuquí y Playa Coquí", foto: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=300&h=200&fit=crop&auto=format", mapsQuery: "Nuquí+Chocó+Colombia" },
    ],
    restaurantes: [
      { nombre: "El Chontaduro", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina afrocolombiana", mapsQuery: "El+Chontaduro+Quibdo+Colombia" },
      { nombre: "La Marimba", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Mariscos del Pacífico", mapsQuery: "La+Marimba+Quibdo" },
    ],
  },
  // ── 7. LETICIA (AMAZONAS) ─────────────────────────────────────────────────
  {
    id: 7, name: "Leticia (Amazonas)", region: "Amazonía", tagline: "La Puerta del Paraíso Amazónico",
    description: "La ciudad más austral de Colombia, en el corazón del Amazonas. Triple frontera con Brasil y Perú, delfines rosados y selva infinita.",
    image: "https://images.unsplash.com/photo-1516026672322-bc52d61a55d5?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1551632811-561732d1e306?w=1200&h=500&fit=crop&auto=format",
    rating: 4.7, weather: "30°C · Tropical", mapsQuery: "Leticia+Amazonas+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Leticia--Colombia/homes",
    historia: "Leticia fue fundada el 25 de abril de 1867 por el militar peruano Manuel Charón y es la capital del departamento de Amazonas, el departamento más extenso de Colombia con 109.665 km². Es la única capital departamental colombiana sin conexión por carretera al resto del país —solo se puede llegar en avión o por el río Amazonas— lo que ha preservado su ecosistema de manera extraordinaria.\n\nEn 1932 fue el epicentro del Conflicto Colombo-Peruano, un breve enfrentamiento armado por el control de la zona que terminó con el retorno del territorio a Colombia en 1933 bajo mediación de la Liga de Naciones. La historia de Leticia es también la historia del caucho: entre 1879 y 1912, la 'fiebre del caucho' devastó las comunidades indígenas amazónicas a través del trabajo forzado.\n\nHoy, Leticia es punto de encuentro de tres países —Colombia, Perú y Brasil— en la llamada Triple Frontera, donde la ciudad brasileña de Tabatinga forma un continuum urbano sin fronteras físicas. El río Amazonas, con más de 6.400 km de longitud, es el rio de mayor caudal del mundo y el corazón de la mayor selva tropical del planeta.",
    cultura: "El Amazonas colombiano es un mosaico de culturas indígenas de una riqueza incomparable. Los Tikuna, el pueblo más numeroso, llevan más de 2.000 años habitando las orillas del Amazonas y son reconocidos por su cerámica pintada con tintes naturales, sus máscaras rituales de corteza de árbol y el ritual de iniciación femenina La Pelazón. Los Yagua se distinguen por sus trajes de fibra de palma y el uso de la cerbatana para la caza; los Bora y los Huitoto por sus cantos rituales y danzas colectivas.\n\nLa selva amazónica no es solo un ecosistema: para los pueblos indígenas es un ser vivo lleno de espíritus, y sus chamanes son los mediadores entre el mundo humano y el espiritual, usando ayahuasca y otras plantas sagradas en sus rituales de curación y visión.\n\nLa gastronomía amazónica sorprende con el pirarucu (el pez de agua dulce más grande del mundo), el mojojoy (larva de palmera asada), el casabe de yuca, el caldo de pirarucú y las frutas tropicales como el copoazú, el açaí, el arazá y el camu camu, que concentran antioxidantes únicos.",
    eventos: [
      { nombre: "Festival de Confraternidad Amazónica", fecha: "Julio 2025", icono: "🌿" },
      { nombre: "Feria Internacional de Leticia", fecha: "Diciembre 2025", icono: "🎁" },
      { nombre: "Semana Cultural Indígena Tikuna", fecha: "Octubre 2025", icono: "🪶" },
    ],
    sitios: [
      { nombre: "Parque Nacional Natural Amacayacu", foto: "https://images.unsplash.com/photo-1516026672322-bc52d61a55d5?w=300&h=200&fit=crop&auto=format", mapsQuery: "Parque+Nacional+Amacayacu+Colombia" },
      { nombre: "Lago Yahuarcaca", foto: "https://images.unsplash.com/photo-1551632811-561732d1e306?w=300&h=200&fit=crop&auto=format", mapsQuery: "Lago+Yahuarcaca+Leticia" },
      { nombre: "Isla de los Micos", foto: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Isla+de+los+Micos+Leticia" },
    ],
    restaurantes: [
      { nombre: "Tierra Amazónica", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina amazónica tradicional", mapsQuery: "Tierra+Amazonica+Leticia" },
      { nombre: "Malabar Amazónico", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Fusión amazónica contemporánea", mapsQuery: "Malabar+Restaurante+Leticia" },
    ],
  },
  // ── 8. SAN ANDRÉS ─────────────────────────────────────────────────────────
  {
    id: 8, name: "San Andrés", region: "Región Insular", tagline: "El Paraíso del Caribe Colombiano",
    description: "Isla coralina de aguas color esmeralda en el Caribe. Reserva de Biosfera UNESCO, buceo de clase mundial y la auténtica cultura raizal.",
    image: "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1618426018046-77564bc4b0eb?w=1200&h=500&fit=crop&auto=format",
    rating: 4.8, weather: "29°C · Soleado", mapsQuery: "San+Andrés+Isla+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/San-Andr%C3%A9s--Colombia/homes",
    historia: "San Andrés fue avistada por Cristóbal Colón en su cuarto viaje en 1502, aunque la colonización efectiva llegó en 1629 con la llegada de colonos puritanos ingleses provenientes de las Islas Bermudas, quienes trajeron consigo esclavos africanos para trabajar en las plantaciones de algodón y tabaco. Esta mezcla de influencias anglosajonas y africanas es la base de la cultura raizal que persiste hasta hoy.\n\nColombia ejerció soberanía efectiva sobre el archipiélago desde 1822, aunque Nicaragua disputó por décadas su pertenencia. En 2012, la Corte Internacional de Justicia de La Haya falló parcialmente a favor de Nicaragua, generando protestas en Colombia. El archipiélago incluye también las islas de Providencia, Santa Catalina y varios cayos e islotes deshabitados.\n\nEn 2000, la UNESCO declaró el Archipiélago de Seaflower Reserva de Biosfera, reconociendo su excepcional biodiversidad marina: el atolón de Providencia tiene el tercer arrecife de coral más extenso del mundo. Hoy, San Andrés recibe más de 500.000 turistas anuales, atraídos por sus aguas turquesas, sus arrecifes vírgenes y el ritmo despreocupado del Caribe.",
    cultura: "Los raizales son el pueblo originario del archipiélago, descendientes de los colonos puritanos ingleses y los esclavos africanos. Su lengua —el creole o kriol— es una mezcla de inglés, africano y español que los distingue del resto de Colombia. Son mayoritariamente protestantes (baptistas y adventistas), lo que marca su cultura y forma de vida de manera profunda, diferenciándola del catolicismo predominante en el resto del país.\n\nLa música raizal bebe del calypso caribeño, el reggae jamaicano y el rhythms and blues afroamericano. La danza schottische y el quadrille son tradiciones folclóricas que se preservan en ceremonias y festivales. La gastronomía de San Andrés es un viaje de sabores: el rondon (sopa de mariscos con leche de coco, ñame y plátano), el rondón de caracol y el pan de coco son platos emblemáticos.\n\nEl snorkel y el buceo son las actividades principales: el Acuario de San Andrés, el arrecife de Jhonny Cay y la pared del Blue Hole son destinos submarinos de fama internacional. La isla está libre de aranceles (duty-free) desde los años 1950, lo que la convierte también en un destino de compras.",
    eventos: [
      { nombre: "Festival del Mar y Coral Festival", fecha: "Junio 2025", icono: "🐠" },
      { nombre: "Festival de la Luna Verde", fecha: "Julio 2025", icono: "🌙" },
      { nombre: "Regata Internacional de Veleros", fecha: "Agosto 2025", icono: "⛵" },
    ],
    sitios: [
      { nombre: "Jhonny Cay y Acuario Natural", foto: "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=300&h=200&fit=crop&auto=format", mapsQuery: "Jhonny+Cay+San+Andres+Colombia" },
      { nombre: "La Cueva de Morgan", foto: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=300&h=200&fit=crop&auto=format", mapsQuery: "Cueva+de+Morgan+San+Andres" },
      { nombre: "Isla de Providencia", foto: "https://images.unsplash.com/photo-1618426018046-77564bc4b0eb?w=300&h=200&fit=crop&auto=format", mapsQuery: "Providencia+Colombia+isla" },
    ],
    restaurantes: [
      { nombre: "La Regatta", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Mariscos y rondon caribeño", mapsQuery: "La+Regatta+San+Andres+Colombia" },
      { nombre: "El Pirata de Morgan", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Cocina raizal y cocteles", mapsQuery: "El+Pirata+de+Morgan+San+Andres" },
    ],
  },
  // ── 9. VILLA DE LEYVA ─────────────────────────────────────────────────────
  {
    id: 9, name: "Villa de Leyva", region: "Boyacá · Andes", tagline: "La Joya Colonial de Boyacá",
    description: "Uno de los pueblos más hermosos de Colombia, con su plaza empedrada más grande del país y un entorno de viñedos y paisajes desérticos.",
    image: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=1200&h=500&fit=crop&auto=format",
    rating: 4.7, weather: "17°C · Soleado", mapsQuery: "Villa+de+Leyva+Boyacá+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Villa-de-Leyva--Colombia/homes",
    historia: "Villa de Leyva fue fundada el 12 de junio de 1572 por el Capitán Hernán Suárez de Villalobos, por orden del Presidente de la Audiencia del Nuevo Reino de Granada, Andrés Díez Venero de Leyva, en cuyo honor lleva su nombre. Desde su fundación, la villa fue escogida como lugar de descanso de la elite colonial granadina gracias a su clima templado y su enclave estratégico en el altiplano boyacense.\n\nEn el siglo XIX, Villa de Leyva fue testigo de importantes páginas de la historia colombiana: el prócer Antonio Nariño estuvo preso aquí, y el sabio Francisco José de Caldas realizó observaciones astronómicas en sus cielos despejados. El 17 de diciembre de 1954, el gobierno de Gustavo Rojas Pinilla declaró a Villa de Leyva Monumento Nacional, protegiendo su arquitectura colonial prácticamente intacta.\n\nHoy, Villa de Leyva conserva una de las plazas empedradas más grandes de América Latina, con más de 14.000 metros cuadrados de adoquines originales. Es uno de los destinos de fin de semana más populares de Colombia, gracias a su cercanía con Bogotá (3.5 horas) y la extraordinaria belleza de su paisaje semiárido.",
    cultura: "Villa de Leyva ofrece una ventana al pasado colonial colombiano en un estado de conservación casi milagroso. Sus calles empedradas, casas blanqueadas con cal y conventos del siglo XVI crean una atmósfera que se antoja salida de un grabado histórico. La Plaza Mayor, con su fuente de piedra del siglo XVII y la imponente Catedral del siglo XVIII, es el corazón de la vida social y el escenario de festivales durante todo el año.\n\nLa región alberga sorpresas naturales únicas: el Desierto de la Candelaria, un microclima semiárido con cactus y tillandsias; el Santuario de Flora y Fauna de Iguaque, con su laguna sagrada donde los Muiscas creían que había emergido la humanidad; y los viñedos de la región, donde se produce uno de los pocos vinos colombianos reconocidos internacionalmente.\n\nEl Parque Paleontológico de Villa de Leyva custodia fósiles de hace 130 millones de años, incluyendo el famoso Kronosauro —el mayor reptil marino hallado en Colombia— y una de las colecciones más importantes de amonites y belemnites de América del Sur.",
    eventos: [
      { nombre: "Festival Astronómico de Villa de Leyva", fecha: "Mayo 2025", icono: "🔭" },
      { nombre: "Festival Internacional de Cometas", fecha: "Agosto 2025", icono: "🪁" },
      { nombre: "Mercado de Pulgas y Antigüedades", fecha: "Todos los fines de semana", icono: "🏺" },
    ],
    sitios: [
      { nombre: "Plaza Mayor (14.000 m²)", foto: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=300&h=200&fit=crop&auto=format", mapsQuery: "Plaza+Mayor+Villa+de+Leyva" },
      { nombre: "Santuario de Fauna e Iguaque", foto: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=300&h=200&fit=crop&auto=format", mapsQuery: "Santuario+Iguaque+Villa+de+Leyva" },
      { nombre: "El Fósil (Plesiosauro)", foto: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=300&h=200&fit=crop&auto=format", mapsQuery: "El+Fosil+Villa+de+Leyva" },
    ],
    restaurantes: [
      { nombre: "Restaurante Savia", foto: "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=300&h=200&fit=crop&auto=format", tipo: "Cocina boyacense contemporánea", mapsQuery: "Savia+Restaurante+Villa+de+Leyva" },
      { nombre: "El Mesón de los Virreyes", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Gastronomía colonial", mapsQuery: "Meson+Virreyes+Villa+de+Leyva" },
    ],
  },
  // ── 10. GUATAPÉ ───────────────────────────────────────────────────────────
  {
    id: 10, name: "Guatapé", region: "Antioquia", tagline: "El Pueblo de los Zócalos",
    description: "Pueblo de colores explosivos a orillas del embalse más grande de Colombia. La Piedra del Peñol vigila el horizonte como un centinela de piedra.",
    image: "https://images.unsplash.com/photo-1545558014-8692077e9b5c?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1653932610416-399b3deb52d2?w=1200&h=500&fit=crop&auto=format",
    rating: 4.8, weather: "20°C · Fresco", mapsQuery: "Guatapé+Antioquia+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Guatap%C3%A9--Colombia/homes",
    historia: "Guatapé fue fundado en 1811 por colonos antioqueños que buscaban tierras fértiles en el oriente de Antioquia. Su historia tomó un giro dramático entre 1967 y 1972, cuando Empresas Públicas de Medellín construyó el embalse Peñol-Guatapé para abastecer de energía eléctrica a Medellín y el norte de Colombia. El embalse inundó la mayor parte del territorio del vecino municipio de El Peñol, y los habitantes debieron trasladar su pueblo a tierra más alta.\n\nEn medio de la construcción, en 1954, surgió el famoso pleito por La Piedra del Peñol: los habitantes de Guatapé pintaron en el monolito las letras 'G' y 'U' para reclamar su pertenencia, pero El Peñol reclamó lo mismo y el mural quedó interrumpido, quedando solo la letra 'G' y parte de la 'U' pintada en la roca —un símbolo visible desde lejos de la disputa que nunca se resolvió del todo.\n\nEl embalse de Guatapé, con 70 km² de superficie, es hoy uno de los destinos turísticos más visitados de Antioquia, con lanchas, canoas y deportes acuáticos en sus aguas tranquilas rodeadas de montañas verdes.",
    cultura: "Lo primero que impacta al llegar a Guatapé son sus zócalos: los relieves en cerámica y escayola que decoran la parte baja de cada fachada del pueblo. Cada casa tiene su propio zócalo que cuenta una historia —la familia que la habita, su oficio, las flores de su jardín— creando un libro de vida colorido que recorre todas las calles del municipio. Esta tradición, única en Colombia, convierte a Guatapé en uno de los pueblos más fotogénicos del país.\n\nLa Piedra del Peñol, un monolito granítico de 200 metros de altura que se eleva abruptamente sobre el embalse, tiene 740 escalones instalados en una grieta natural que permiten subir a la cima. La vista desde arriba —laberinto de islas, penínsulas y agua turquesa rodeado de montañas— es considerada una de las más espectaculares de Colombia.\n\nLa gastronomía de Guatapé gira alrededor de los productos del embalse: la trucha del Peñol, el plátano maduro asado y los tamales antioqueños son los platos más representativos. Los fines de semana, el malecón es un espectáculo de familias, lanchas, músicos y coloridos restaurantes sobre el agua.",
    eventos: [
      { nombre: "Festival de la Cultura y Zócalos", fecha: "Agosto 2025", icono: "🎨" },
      { nombre: "Festival del Agua y el Embalse", fecha: "Junio 2025", icono: "💧" },
      { nombre: "Triatlón del Embalse Peñol-Guatapé", fecha: "Octubre 2025", icono: "🏊" },
    ],
    sitios: [
      { nombre: "La Piedra del Peñol (740 escalones)", foto: "https://images.unsplash.com/photo-1545558014-8692077e9b5c?w=300&h=200&fit=crop&auto=format", mapsQuery: "La+Piedra+del+Peñol+Colombia" },
      { nombre: "Malecón de Guatapé", foto: "https://images.unsplash.com/photo-1653932610416-399b3deb52d2?w=300&h=200&fit=crop&auto=format", mapsQuery: "Malecon+Guatapé+Antioquia" },
      { nombre: "Zócalos y Centro Histórico", foto: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=300&h=200&fit=crop&auto=format", mapsQuery: "Centro+Historico+Guatapé+Colombia" },
    ],
    restaurantes: [
      { nombre: "La Mesa de Pedro", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Trucha y cocina antioqueña", mapsQuery: "La+Mesa+de+Pedro+Guatapé" },
      { nombre: "El Rancho del Embalse", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Parrilla con vista al lago", mapsQuery: "Rancho+del+Embalse+Guatapé" },
    ],
  },
  // ── 11. SALENTO ───────────────────────────────────────────────────────────
  {
    id: 11, name: "Salento", region: "Quindío · Eje Cafetero", tagline: "Corazón del Paisaje Cultural Cafetero",
    description: "Pueblo de colores vibrantes en el corazón del Eje Cafetero. Palmas de cera, cafetales y cultura cafetera UNESCO.",
    image: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=1200&h=500&fit=crop&auto=format",
    rating: 4.9, weather: "19°C · Fresco", mapsQuery: "Salento+Quindío+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Salento--Colombia/homes",
    historia: "Salento es el municipio más antiguo del Quindío, fundado en 1842 como campamento base de los colonizadores antioqueños que avanzaban hacia el sur en lo que se conoció como la Colonización Antioqueña. Su nombre original fue 'Boquía', pero fue rebautizado en honor a la ciudad italiana de Salerno por sus fundadores. La cepa poblacional paisa que colonizó esta región construyó una identidad propia basada en el trabajo de la tierra, la hospitalidad y el cultivo del café.\n\nEn 2011, la UNESCO declaró el Paisaje Cultural Cafetero de Colombia —que incluye a Salento y su entorno— Patrimonio de la Humanidad, reconociendo la excepcional interacción entre la cultura humana y el medio natural que ha dado lugar a una forma de vida única en el mundo. Este reconocimiento convirtió a Salento en uno de los destinos más visitados de Colombia.\n\nEl Valle del Cocora, a pocos kilómetros de Salento, alberga la palma de cera (Ceroxylon quindiuense), el árbol nacional de Colombia y la palmera más alta del mundo, que puede alcanzar los 60 metros de altura en las nieblas del Andes. Esta especie está en peligro de extinción y la palma de cera de Cocora es un símbolo identitario de Colombia.",
    cultura: "La vida en Salento gira alrededor del café: el ritual del lavado, secado y tostado del grano; las visitas a las fincas cafeteras donde los recolectores recogen el grano a mano; los beneficiaderos donde se procesa el café fresco; y finalmente la taza perfecta servida en una terraza con vista a los cafetales. Los turistas pueden participar en recorridos completos de finca cafetera y aprender todo el proceso de la semilla a la taza.\n\nSalento es también la capital del bahareque —técnica de construcción tradicional con guadua (bambú gigante) y barro que ha resistido terremotos durante siglos— y sus coloridas construcciones han inspirado a arquitectos de todo el mundo. Sus balcones llenos de flores, la calle Real con sus tiendas de artesanías y sus restaurantes de trucha son elementos inseparables del encanto del pueblo.\n\nLa gastronomía del Eje Cafetero tiene personalidad propia: la trucha criada en las frías quebradas de montaña, el bandeja paisa modernizada, las obleas con arequipe, el changua (sopa de leche con huevo) y el café de origen son experiencias gastronómicas que no se encuentran igual en ningún otro lugar del mundo.",
    eventos: [
      { nombre: "Festival de la Trova y Bandola", fecha: "Julio 2025", icono: "🎸" },
      { nombre: "Feria Nacional del Café", fecha: "Mayo 2025", icono: "☕" },
      { nombre: "Festival de Cometas Valle del Cocora", fecha: "Agosto 2025", icono: "🪁" },
    ],
    sitios: [
      { nombre: "Valle del Cocora (Palmas de Cera)", foto: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=300&h=200&fit=crop&auto=format", mapsQuery: "Valle+del+Cocora+Salento+Colombia" },
      { nombre: "Finca Cafetera La Esperanza", foto: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Finca+Cafetera+Salento+Quindio" },
      { nombre: "Calle Real de Salento", foto: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=300&h=200&fit=crop&auto=format", mapsQuery: "Calle+Real+Salento+Colombia" },
    ],
    restaurantes: [
      { nombre: "La Eliana Restaurante", foto: "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=300&h=200&fit=crop&auto=format", tipo: "Trucha del Quindío y cocina paisa", mapsQuery: "La+Eliana+Salento+Quindio" },
      { nombre: "Veintiuno Café", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Café de especialidad y pasteles", mapsQuery: "Veintiuno+Cafe+Salento" },
    ],
  },
  // ── 12. POPAYÁN ───────────────────────────────────────────────────────────
  {
    id: 12, name: "Popayán", region: "Cauca", tagline: "La Ciudad Blanca",
    description: "La Ciudad Blanca de Colombia, con su arquitectura colonial encalada, célebres procesiones de Semana Santa y gastronomía reconocida por la UNESCO.",
    image: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=1200&h=500&fit=crop&auto=format",
    rating: 4.6, weather: "17°C · Fresco", mapsQuery: "Popayán+Cauca+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Popay%C3%A1n--Colombia/homes",
    historia: "Popayán fue fundada el 13 de enero de 1537 por Sebastián de Belalcázar durante su expedición desde Quito hacia el norte del continente. Desde su fundación fue la sede del Gobierno de Popayán, que controlaba vastos territorios en el sur de Colombia y el norte de Ecuador. Su posición estratégica en la ruta entre Lima y Bogotá la convirtió en una de las ciudades más importantes del Virreinato de la Nueva Granada.\n\nLa ciudad fue devastada por un terremoto el 31 de marzo de 1983, precisamente en Jueves Santo, cuando miles de fieles participaban en las tradicionales procesiones de Semana Santa. El sismo destruyó gran parte del patrimonio histórico colonial, pero la ciudad fue reconstruida con tal fidelidad a sus originales que su blancura y perfección arquitectónica son hoy aún más notables.\n\nEn 2005, la UNESCO reconoció a Popayán como Ciudad Creativa de la Gastronomía —la primera en América Latina en recibir este reconocimiento— por la extraordinaria riqueza de su cocina tradicional caucana que fusiona ingredientes y técnicas indígenas, africanas y españolas en preparaciones únicas que se transmiten de generación en generación.",
    cultura: "Las procesiones de Semana Santa de Popayán, celebradas ininterrumpidamente desde 1558, son el corazón espiritual y cultural de la ciudad. Por siglos, hermandades de capuchones silenciosos cargan pasos monumentales con esculturas religiosas de los siglos XVII y XVIII en noches iluminadas por miles de velas. En 2009, la UNESCO las declaró Patrimonio Cultural Inmaterial de la Humanidad, reconociendo su excepcional continuidad histórica.\n\nLa cocina caucana es una de las más diversas y complejas de Colombia: el pipián (salsa de maní y hierba santa), el mote de queso, el masato de maíz, el champús (bebida de maíz, lulo y panela) y el tamal caucano son especialidades que los chefs payaneses reinterpretan con técnicas modernas en un movimiento gastronómico de reconocimiento internacional.\n\nPopayán ha sido cuna de 17 presidentes de Colombia y de figuras como el General Francisco de Paula Santander. Su Universidad del Cauca, fundada en 1827, es una de las más antiguas del país y confiere a la ciudad un ambiente universitario y cultural vivaz. El Museo Arqueológico y el Museo de Historia Natural albergan colecciones únicas sobre la cultura precolombina del Cauca.",
    eventos: [
      { nombre: "Semana Santa de Popayán (UNESCO)", fecha: "Abril 2025", icono: "⛪" },
      { nombre: "Congreso Gastronómico Nacional", fecha: "Septiembre 2025", icono: "🍽️" },
      { nombre: "Festival de Música Religiosa", fecha: "Abril 2025", icono: "🎼" },
    ],
    sitios: [
      { nombre: "Catedral de Nuestra Señora de la Asunción", foto: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=300&h=200&fit=crop&auto=format", mapsQuery: "Catedral+Popayan+Cauca" },
      { nombre: "Puente del Humilladero", foto: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=300&h=200&fit=crop&auto=format", mapsQuery: "Puente+del+Humilladero+Popayan" },
      { nombre: "Torre del Reloj y Centro Histórico", foto: "https://images.unsplash.com/photo-1778188985186-25a9d5f7d2b8?w=300&h=200&fit=crop&auto=format", mapsQuery: "Torre+del+Reloj+Popayan" },
    ],
    restaurantes: [
      { nombre: "Mora Castilla", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Alta cocina caucana", mapsQuery: "Mora+Castilla+Restaurante+Popayan" },
      { nombre: "La Fresa", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Gastronomía tradicional caucana", mapsQuery: "La+Fresa+Restaurante+Popayan" },
    ],
  },
  // ── 13. SAN AGUSTÍN ───────────────────────────────────────────────────────
  {
    id: 13, name: "San Agustín", region: "Huila", tagline: "El Misterio de los Dioses de Piedra",
    description: "El sitio arqueológico precolombino más importante de América del Sur, con más de 500 estatuas funerarias de piedra talladas por una civilización misteriosa.",
    image: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=1200&h=500&fit=crop&auto=format",
    rating: 4.7, weather: "16°C · Fresco", mapsQuery: "San+Agustín+Huila+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/San-Agust%C3%ADn--Colombia/homes",
    historia: "El Parque Arqueológico de San Agustín fue declarado Patrimonio de la Humanidad por la UNESCO en 1995, reconociendo la excepcional importancia de su colección de esculturas y tumbas precolombinas. La cultura agustiniana floreció entre los siglos I y VIII d.C. en las cabeceras de los ríos Magdalena y Cauca, en el corazón del macizo colombiano, y dejó un legado monumental sin precedentes en América precolombina.\n\nSe han descubierto más de 500 estatuas de piedra con rasgos zoomorfos y antropomorfos que representan dioses, guerreros y chamanes en actitudes rituales. Las esculturas, que pueden medir desde pocos centímetros hasta más de 4 metros de altura, fueron talladas en andesita volcánica con herramientas de piedra, lo que hace aún más impresionante su acabado y expresividad.\n\nLo más misterioso de esta civilización es que desapareció sin dejar rastro escrito: no sabemos su nombre, su lengua ni las razones de su desaparición. Los pueblos indígenas actuales de la región —los Yanacona— no tienen memoria ancestral directa de los constructores de estas maravillas, que permanecen como un enigma fascinante para la arqueología mundial.",
    cultura: "San Agustín y su entorno inmediato —incluyendo el municipio de Isnos— concentran mesitas, colinas artificiales y túmulos funerarios que forman necrópolis complejas donde los jefes y chamanes fueron enterrados con sus ajuares y protegidos por guardianes de piedra. La Fuente de Lavapatas, excavada en el lecho del río, es un sistema de canales, piscinas y figuras talladas directamente en la roca basáltica por donde fluía el agua en rituales sagrados.\n\nEl macizo colombiano donde se asienta San Agustín es el 'estrella fluvial' de Colombia: desde aquí nacen los cuatro principales ríos del país —el Magdalena, el Cauca, el Patía y el Caquetá. Este rasgo geográfico excepcional probablemente tuvo un significado sagrado para los agustinianos, que ubicaron sus centros ceremoniales en este punto donde el agua brota en múltiples direcciones.\n\nLa región ofrece también turismo de aventura: el río Magdalena, en sus primeros kilómetros desde su nacimiento, es perfecto para el rafting; las cascadas del Duende y el Bordones ofrecen caminatas espectaculares entre vegetación de bosque andino; y las fincas del municipio producen café de altísima calidad que puede degustarse directamente en su lugar de origen.",
    eventos: [
      { nombre: "Festival de Músicas del Mundo", fecha: "Junio 2025", icono: "🌍" },
      { nombre: "Carnaval del Perdón y del Arco Iris", fecha: "Febrero 2025", icono: "🌈" },
      { nombre: "Feria del Café y la Cultura Agustiniana", fecha: "Septiembre 2025", icono: "☕" },
    ],
    sitios: [
      { nombre: "Parque Arqueológico Mesita A, B, C y D", foto: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Parque+Arqueologico+San+Agustin" },
      { nombre: "Fuente de Lavapatas", foto: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=300&h=200&fit=crop&auto=format", mapsQuery: "Fuente+de+Lavapatas+San+Agustin" },
      { nombre: "Cascada del Bordones", foto: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=300&h=200&fit=crop&auto=format", mapsQuery: "Cascada+Bordones+San+Agustin" },
    ],
    restaurantes: [
      { nombre: "Restaurante El Fogón", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Cocina huilense tradicional", mapsQuery: "El+Fogon+Restaurante+San+Agustin" },
      { nombre: "Hospedería y Restaurante Akawanka", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Gastronomía local con café", mapsQuery: "Akawanka+San+Agustin+Huila" },
    ],
  },
  // ── 14. MOMPOX ────────────────────────────────────────────────────────────
  {
    id: 14, name: "Mompox", region: "Bolívar · Río Magdalena", tagline: "La Ciudad que el Tiempo Olvidó",
    description: "Ciudad colonial del siglo XVI incrustada en una isla del Río Magdalena. Patrimonio UNESCO, procesiones de Semana Santa y filigrana de oro milenaria.",
    image: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=1200&h=500&fit=crop&auto=format",
    rating: 4.7, weather: "33°C · Cálido", mapsQuery: "Mompox+Bolívar+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Mompox--Colombia/homes",
    historia: "Santa Cruz de Mompox fue fundada en 1537 por Alonso de Heredia, hermano del fundador de Cartagena. Su posición sobre una isla en el río Magdalena la convirtió en el centro comercial más importante del norte de Colombia durante los siglos XVI, XVII y XVIII: por aquí pasaba todo el comercio entre el interior del continente y el puerto de Cartagena. El oro, las esmeraldas y los productos del interior americano fluían por el Magdalena hacia Mompox y de ahí a Europa.\n\nSimón Bolívar reclutó en Mompox los soldados que lucharían por la Independencia, y fue aquí donde pronunció su célebre frase: 'Si a Caracas le debo la vida, a Mompox le debo la gloria'. En 1810, Mompox fue la primera ciudad del Caribe colombiano en declarar su independencia de España.\n\nEn 1995, la UNESCO declaró a Mompox Patrimonio de la Humanidad, reconociendo la extraordinaria conservación de su arquitectura colonial. A diferencia de otras ciudades históricas que han sido modernizadas, Mompox permanece casi intacta desde el siglo XVIII, pues la apertura del Canal del Dique desvió el comercio hacia Barranquilla y la ciudad quedó sumida en un olvido que, paradójicamente, preservó su patrimonio.",
    cultura: "Mompox es famosa en Colombia por sus procesiones de Semana Santa, que compiten en antigüedad y solemnidad con las de Popayán. Por las noches del Miércoles, Jueves y Viernes Santos, hermandades de penitentes con túnicas blancas llevan en silencio pasos de imágenes sacras por las calles empedradas iluminadas con velas, en un ritual que se ha mantenido ininterrumpido durante más de cuatro siglos.\n\nLa filigrana de oro momposina es otra joya cultural: los artesanos de la ciudad trabajan el oro en filamentos finísimos, entretejiendo intrincados diseños que reproducen figuras naturales e históricas. Esta técnica, que requiere décadas de aprendizaje, produce las piezas de joyería más delicadas de Colombia y se transmite de padres a hijos en talleres del barrio histórico.\n\nMompox vive a ritmo lento: las tardes de calor extremo invitan a la siesta bajo los techos de teja que protegen los amplios zaguanes coloniales, mientras los pájaros y el murmullo del río marcan el tempo de una vida que parece detenida en el tiempo. El escritor Gabriel García Márquez amó esta ciudad e inspiró en ella la Macondo de 'Cien Años de Soledad'.",
    eventos: [
      { nombre: "Semana Santa de Mompox", fecha: "Abril 2025", icono: "🕯️" },
      { nombre: "Festival Cultural de Mompox", fecha: "Octubre 2025", icono: "🎭" },
      { nombre: "Encuentro Nacional de Filigrana", fecha: "Noviembre 2025", icono: "💍" },
    ],
    sitios: [
      { nombre: "Calle de la Albarrada", foto: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=300&h=200&fit=crop&auto=format", mapsQuery: "Calle+Albarrada+Mompox" },
      { nombre: "Iglesias Coloniales del Centro", foto: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=300&h=200&fit=crop&auto=format", mapsQuery: "Iglesia+Santa+Barbara+Mompox" },
      { nombre: "Cementerio Central y Talleres de Filigrana", foto: "https://images.unsplash.com/photo-1778188985186-25a9d5f7d2b8?w=300&h=200&fit=crop&auto=format", mapsQuery: "Cementerio+Mompox+Colombia" },
    ],
    restaurantes: [
      { nombre: "Comedor Costeño", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina costeña del Magdalena", mapsQuery: "Comedor+Costeno+Mompox" },
      { nombre: "El Fuerte", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Carnes y pescados del río", mapsQuery: "El+Fuerte+Restaurante+Mompox" },
    ],
  },
  // ── 15. CABO DE LA VELA ───────────────────────────────────────────────────
  {
    id: 15, name: "Cabo de la Vela", region: "La Guajira", tagline: "El Fin del Mundo Colombiano",
    description: "En la árida Guajira, donde el desierto se funde con el mar Caribe. Territorio sagrado Wayuu, kitesurf y los atardeceres más dramáticos de Colombia.",
    image: "https://images.unsplash.com/photo-1559827291-72ee739d0d9a?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?w=1200&h=500&fit=crop&auto=format",
    rating: 4.8, weather: "35°C · Ventoso", mapsQuery: "Cabo+de+la+Vela+La+Guajira+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Cabo-de-la-Vela--Colombia/homes",
    historia: "Cabo de la Vela fue el primer punto del continente suramericano avistado por Alonso de Ojeda y Américo Vespucio en 1499, quienes doblaron el promontorio y le dieron su nombre porque la silueta del cerro se asemejaba a una vela desplegada. Este hecho geográfico convirtió al cabo en el primer suelo continental de América del Sur conocido por europeos.\n\nLa península de La Guajira, donde se ubica Cabo de la Vela, ha sido territorio del pueblo Wayuu durante milenios. Los Wayuu, que se llaman a sí mismos 'Wayuu Aapüshi' (pueblo del viento), son el grupo indígena más numeroso de Colombia con más de 300.000 personas, y su territorio abarca tanto el norte de Colombia como el noroeste de Venezuela. Lograron resistir la colonización española durante siglos gracias a su organización matrilineal en clanes y a la aridez de su territorio, que desanimaba a los conquistadores.\n\nHoy, Cabo de la Vela y la cercana Punta Gallinas —el punto más norte de América del Sur— son destinos de ecoturismo de aventura que reciben viajeros de todo el mundo atraídos por la excepcional belleza de su paisaje desértico y sus aguas azul cobalto.",
    cultura: "Los Wayuu son los señores del viento y del desierto. Su sociedad matrilineal organizada en clanes familiares, su lengua propia (el wayuunaiki), sus tejidos de mochila (considerados entre los más complejos del mundo), sus rituales del sueño y su gastronomía de chivo seco y friche representan una de las culturas indígenas más vivas y orgullosas de Colombia.\n\nLa mochila wayuu —tejida por las mujeres en patrones geométricos que representan el clan familiar y cosmovisión de la tejedora— es hoy un símbolo de la moda colombiana en pasarelas internacionales, aunque su significado va más allá de la estética: es un mapa identitario, una historia y una forma de comunicación visual.\n\nEl kitesurf en Cabo de la Vela es de clase mundial: los vientos alisios constantes (30-60 km/h) durante gran parte del año crean condiciones perfectas para el deporte. El atardecer sobre el mar desde el Pilón de Azúcar —un promontorio rocoso sagrado para los Wayuu— es considerado uno de los espectáculos naturales más impresionantes de Colombia.",
    eventos: [
      { nombre: "Encuentro Cultural Wayuu", fecha: "Mayo 2025", icono: "🪶" },
      { nombre: "Competencia de Kitesurf Cabo de la Vela", fecha: "Febrero 2025", icono: "🪁" },
      { nombre: "Fiesta de la Virgen del Pilar", fecha: "Octubre 2025", icono: "⛵" },
    ],
    sitios: [
      { nombre: "Punta Gallinas (Extremo Norte de Sudamérica)", foto: "https://images.unsplash.com/photo-1559827291-72ee739d0d9a?w=300&h=200&fit=crop&auto=format", mapsQuery: "Punta+Gallinas+La+Guajira+Colombia" },
      { nombre: "Pilón de Azúcar", foto: "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?w=300&h=200&fit=crop&auto=format", mapsQuery: "Pilon+de+Azucar+Cabo+de+la+Vela" },
      { nombre: "Playa del Cabo (Kitesurf)", foto: "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=300&h=200&fit=crop&auto=format", mapsQuery: "Playa+Cabo+de+la+Vela+Colombia" },
    ],
    restaurantes: [
      { nombre: "Ranchería Wayuu Utta", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina Wayuu: chivo y friche", mapsQuery: "Rancheria+Wayuu+Cabo+de+la+Vela" },
      { nombre: "El Faro Azul", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Mariscos en el desierto", mapsQuery: "El+Faro+Azul+Cabo+de+la+Vela" },
    ],
  },
  // ── 16. CAÑO CRISTALES ────────────────────────────────────────────────────
  {
    id: 16, name: "Caño Cristales", region: "Meta · Serranía de la Macarena", tagline: "El Río de los Cinco Colores",
    description: "El río más hermoso del mundo: un arcoíris líquido con algas multicolores de rojo, amarillo, verde, azul y negro en el corazón de Colombia.",
    image: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1516026672322-bc52d61a55d5?w=1200&h=500&fit=crop&auto=format",
    rating: 4.9, weather: "28°C · Tropical", mapsQuery: "Caño+Cristales+La+Macarena+Meta+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/La-Macarena--Colombia/homes",
    historia: "Caño Cristales, ubicado en el Parque Nacional Natural Sierra de la Macarena en el departamento del Meta, fue denominado por los viajeros como 'el río más hermoso del mundo', 'el río de los cinco colores' y 'el arcoíris derretido'. Su excepcional colorido se debe a la presencia de la planta acuática Macarenia clavigera, una alga endémica que en su temporada de floración (julio-noviembre) tiñe el lecho del río de un rojo intenso que contrasta con los amarillos de otras algas, los verdes del musgo, los azules del cielo reflejado y los negros del basalto.\n\nEl Parque Nacional Natural Sierra de la Macarena fue declarado Reserva de la Biosfera por la UNESCO en 1979, reconociendo que esta formación geológica es un punto de confluencia de cuatro grandes ecosistemas colombianos: la Amazonía, la Orinoquía, los Andes y el Chocó biogeográfico. Esta confluencia única de biomas produce una biodiversidad extraordinaria: más de 500 especies de aves, 50 de reptiles y 150 de mariposas.\n\nEl acceso turístico a Caño Cristales fue prohibido entre 1989 y 2009 por el conflicto armado, lo que paradójicamente preservó el ecosistema en condiciones pristinas. Su reapertura al turismo regulado ha convertido a La Macarena en uno de los destinos de naturaleza más deseados del mundo.",
    cultura: "La Serranía de la Macarena es un laboratorio natural de evolución: aislada geológicamente durante millones de años, ha desarrollado especies que no existen en ningún otro lugar del planeta. Los tepuyes (mesetas de arenisca precámbrica) y el basalto del río crean microhábitats únicos donde las condiciones de luz, humedad y corriente producen el espectáculo cromático de Caño Cristales.\n\nEl pueblo de La Macarena, la única localidad cercana al río, ha basado su economía en el ecoturismo con guías locales certificados que acompañan a los visitantes por rutas diseñadas para minimizar el impacto ambiental. El turismo está estrictamente regulado: solo se permite el ingreso a grupos reducidos, no se puede nadar en algunas áreas para proteger las algas, y los visitantes deben seguir rutas marcadas con el apoyo de guías nativos.\n\nLa mejor época para visitar es entre septiembre y noviembre, cuando el nivel del agua es perfecto para que las algas se tiñan de su rojo más intenso. Las piscinas naturales del río, con sus formas caprichosas esculpidas en la roca durante millones de años, son piscinas naturales de cristal turquesa donde el baño es una experiencia mágica.",
    eventos: [
      { nombre: "Temporada de Colores (algas en flor)", fecha: "Septiembre-Noviembre 2025", icono: "🌈" },
      { nombre: "Festival Ecológico de La Macarena", fecha: "Octubre 2025", icono: "🌿" },
      { nombre: "Feria del Visitante La Macarena", fecha: "Agosto 2025", icono: "🦋" },
    ],
    sitios: [
      { nombre: "Caño Cristales (Río de los Cinco Colores)", foto: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Caño+Cristales+La+Macarena" },
      { nombre: "Piscinas Naturales y Pozos Azules", foto: "https://images.unsplash.com/photo-1516026672322-bc52d61a55d5?w=300&h=200&fit=crop&auto=format", mapsQuery: "Pozos+Azules+Caño+Cristales" },
      { nombre: "Cascadas del Río Cristales", foto: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=300&h=200&fit=crop&auto=format", mapsQuery: "Cascadas+Rio+Cristales+Meta" },
    ],
    restaurantes: [
      { nombre: "Rancho Macarena", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Cocina llanera y amazónica", mapsQuery: "Rancho+Macarena+La+Macarena+Meta" },
      { nombre: "El Güio Restaurante", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Parrilla al carbón y peces de río", mapsQuery: "El+Guio+La+Macarena+Meta" },
    ],
  },
  // ── 17. TATACOA ───────────────────────────────────────────────────────────
  {
    id: 17, name: "Desierto de la Tatacoa", region: "Huila · Villavieja", tagline: "El Desierto Más Hermoso de Colombia",
    description: "Un desierto de tonos rojos y grises con cactus gigantes, fósiles prehistóricos y los cielos más estrellados de Colombia para la observación astronómica.",
    image: "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1559827291-72ee739d0d9a?w=1200&h=500&fit=crop&auto=format",
    rating: 4.6, weather: "38°C · Árido", mapsQuery: "Desierto+de+la+Tatacoa+Huila+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Villavieja--Colombia/homes",
    historia: "El Desierto de la Tatacoa, ubicado a 38 km de Neiva en el municipio de Villavieja, es en realidad un bosque tropical seco —el segundo ecosistema más amenazado del planeta después del bosque tropical húmedo— donde la falta de lluvia y la evapotranspiración extrema han creado un paisaje desértico de 330 km² de arcillas erosionadas. La palabra 'Tatacoa' proviene del español y hace referencia a las serpientes que habitaban la zona, aunque hoy 'mataratón' y 'cardón' son las plantas más características.\n\nHace entre 10 y 12 millones de años, esta región estuvo cubierta por bosques tropicales húmedos habitados por megafauna: mastodontes, gliptodontes, toxodontes y caimanes prehistóricos, cuyos fósiles siguen apareciendo hoy en la superficie erosionada del desierto tras cada lluvia. El Museo de Paleontología de Villavieja custodia una de las colecciones paleontológicas más importantes de Suramérica.\n\nEl geógrafo colombiano Francisco José de Caldas fue el primero en describir científicamente la Tatacoa en el siglo XIX. Hoy, el Observatorio Astronómico de la Tatacoa, construido en 1991, es uno de los más importantes de Colombia gracias a la baja humedad y la escasa contaminación lumínica que permiten observar el cielo estrellado con una claridad extraordinaria durante gran parte del año.",
    cultura: "La Tatacoa hipnotiza con su paleta de colores imposibles: el Cuzco es la zona de arcillas rojas con farallones y cañones esculpidos por el agua durante millones de años; Los Hoyos es la zona gris-azulada con laberintos de formaciones calcáreas parecidas a un paisaje lunar. El contraste entre ambas zonas hace de la Tatacoa un destino fotográfico de primer orden, frecuentado por fotógrafos de paisaje de todo el mundo.\n\nLa vida en la Tatacoa es extraordinariamente adaptada al calor extremo: las iguanas, las serpientes, los zorros, los conejos silvestres y la abundante avifauna —más de 150 especies de aves registradas— han desarrollado estrategias de supervivencia fascinantes que el turismo de naturaleza permite observar de cerca.\n\nLa noche en la Tatacoa es su momento más mágico: sin contaminación lumínica y con baja humedad, la Vía Láctea se despliega con una nitidez asombrosa sobre el horizonte desértico. El Observatorio ofrece sesiones nocturnas con telescopios donde se pueden observar nebulosas, cúmulos estelares y planetas con detalles extraordinarios. La combinación de desierto, fósiles y astros hace de la Tatacoa uno de los destinos más únicos y menos masificados de Colombia.",
    eventos: [
      { nombre: "Festival de Astronomía de la Tatacoa", fecha: "Septiembre 2025", icono: "🔭" },
      { nombre: "Festival del Merecumbé (danza huilense)", fecha: "Junio 2025", icono: "💃" },
      { nombre: "Expedición Paleontológica con guías", fecha: "Todo el año", icono: "🦕" },
    ],
    sitios: [
      { nombre: "Zona Roja (El Cuzco)", foto: "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?w=300&h=200&fit=crop&auto=format", mapsQuery: "Tatacoa+Roja+Huila+Colombia" },
      { nombre: "Zona Gris (Los Hoyos)", foto: "https://images.unsplash.com/photo-1559827291-72ee739d0d9a?w=300&h=200&fit=crop&auto=format", mapsQuery: "Tatacoa+Gris+Villavieja+Huila" },
      { nombre: "Observatorio Astronómico de la Tatacoa", foto: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Observatorio+Astronomico+Tatacoa" },
    ],
    restaurantes: [
      { nombre: "Restaurante La Piscina Tatacoa", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Asado huilense y jugos naturales", mapsQuery: "Restaurante+Tatacoa+Villavieja" },
      { nombre: "El Cardón Comedor", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina campesina del Huila", mapsQuery: "El+Cardon+Tatacoa+Huila" },
    ],
  },
  // ── 18. CALI ──────────────────────────────────────────────────────────────
  {
    id: 18, name: "Cali", region: "Valle del Cauca", tagline: "La Capital Mundial de la Salsa",
    description: "La ciudad del ritmo, la alegría y el Pacífico. Cali baila salsa 24 horas al día en la tierra de los campeones mundiales de baile.",
    image: "https://images.unsplash.com/photo-1653932610416-399b3deb52d2?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1515366974328-f1181eb25189?w=1200&h=500&fit=crop&auto=format",
    rating: 4.7, weather: "24°C · Cálido", mapsQuery: "Cali+Valle+del+Cauca+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Cali--Colombia/homes",
    historia: "Cali fue fundada el 25 de julio de 1536 por Sebastián de Belalcázar, convirtiéndose en una de las primeras ciudades fundadas por los españoles en el Pacífico suramericano. Durante la época colonial fue un importante nodo en la ruta entre el Pacífico y el interior del continente: el puerto de Buenaventura, a 120 km, canalizaba el comercio con Asia y Europa.\n\nEl cultivo de la caña de azúcar transformó el Valle del Cauca en el siglo XX en la región agroindustrial más próspera de Colombia, con los ingenios azucareros que emplearon mano de obra afrodescendiente traída del Pacífico colombiano. Esta migración masiva trajo la música, la gastronomía y la cultura del Pacífico a la ciudad, gestando la mezcla cultural única que daría origen a la salsa caleña.\n\nLos Juegos Panamericanos de 1971 convirtieron a Cali en vitrina continental y dejaron infraestructura deportiva de primer nivel. Hoy, con 2,5 millones de habitantes, es la tercera ciudad más grande de Colombia y se proyecta como polo de negocios del Pacífico colombiano.",
    cultura: "La salsa caleña es una religión. El estilo caleño —reconocido internacionalmente como el más limpio, rápido y técnico del mundo— se distingue del cubano, el neoyorquino y el puertorriqueño por su trabajo de pies sobre la beat, sus figuras aéreas y su energía explosiva. Los salsódromos, las academias de baile, los campeonatos mundiales y el Festival Mundial de Salsa de agosto hacen de Cali la meca global del género.\n\nEl barrio San Antonio, con sus casas de bahareque del siglo XIX en colores pastel, es el corazón bohemio y cultural de la ciudad: cafés, galerías, tiendas de vinilo y la Iglesia de San Antonio en la colina son sus principales atractivos. El Cristo Rey, de 26 metros de altura en la cima de los cerros, vigila la ciudad como un guardián permanente.\n\nLa gastronomía caleña es el encuentro del Valle fértil con el Pacífico: el sancocho de gallina criolla, el champús, el lulada, el chontaduro con miel, la cholada y la fritanga son platos que se disfrutan en el calor tropical de una ciudad que festeja con igual intensidad los triunfos deportivos, los campeonatos de salsa y las fiestas decembrinas de la Feria de Cali.",
    eventos: [
      { nombre: "Feria de Cali (26-31 Diciembre)", fecha: "Diciembre 2025", icono: "💃" },
      { nombre: "Festival Mundial de Salsa", fecha: "Agosto 2025", icono: "🎺" },
      { nombre: "Cali Viejo Festival Jazz", fecha: "Mayo 2025", icono: "🎷" },
    ],
    sitios: [
      { nombre: "Cristo Rey de Cali", foto: "https://images.unsplash.com/photo-1653932610416-399b3deb52d2?w=300&h=200&fit=crop&auto=format", mapsQuery: "Cristo+Rey+Cali+Colombia" },
      { nombre: "Barrio San Antonio", foto: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=300&h=200&fit=crop&auto=format", mapsQuery: "Barrio+San+Antonio+Cali" },
      { nombre: "Zoológico de Cali", foto: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Zoologico+de+Cali+Colombia" },
    ],
    restaurantes: [
      { nombre: "Platillos Voladores", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Fusión valluna-pacífica", mapsQuery: "Platillos+Voladores+Cali" },
      { nombre: "El Buen Alimento", foto: "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=300&h=200&fit=crop&auto=format", tipo: "Gastronomía caleña tradicional", mapsQuery: "El+Buen+Alimento+Cali+Colombia" },
    ],
  },
  // ── 19. BARRANQUILLA ──────────────────────────────────────────────────────
  {
    id: 19, name: "Barranquilla", region: "Atlántico · Costa Caribe", tagline: "La Ciudad del Eterno Carnaval",
    description: "El Carnaval más grande de Colombia y segundo de Latinoamérica. Ciudad Patrimonio Cultural UNESCO donde el Caribe explota en cumbia, vallenato y mapalé.",
    image: "https://images.unsplash.com/photo-1618426018046-77564bc4b0eb?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=1200&h=500&fit=crop&auto=format",
    rating: 4.6, weather: "30°C · Caluroso", mapsQuery: "Barranquilla+Atlántico+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Barranquilla--Colombia/homes",
    historia: "Barranquilla fue fundada a principios del siglo XVII de manera espontánea —sin acto fundacional formal— como un puerto en la desembocadura del río Magdalena al mar Caribe, lo que la convirtió en la puerta de entrada comercial de Colombia al mundo. Gracias a su posición privilegiada, se convirtió en el primer puerto de Colombia y en el centro industrial y comercial más importante del país a principios del siglo XX.\n\nLa aviación colombiana nació en Barranquilla: la empresa SCADTA, fundada en 1919, fue la segunda aerolínea del mundo en operar rutas comerciales regulares. Shakiré (Shakira), la artista colombiana más exitosa del mundo, nació en Barranquilla en 1977, y la ciudad celebra con orgullo a su hija más famosa con una estatua en el Puerto Colombia.\n\nEl Carnaval de Barranquilla fue declarado Patrimonio Oral e Inmaterial de la Humanidad por la UNESCO en 2003, reconociendo que reúne más de 300 danzas tradicionales que sintetizan las herencias indígenas, africanas y españolas de Colombia en cuatro días de festividad masiva que moviliza a más de 1,5 millones de personas cada año.",
    cultura: "El Carnaval de Barranquilla celebra la identidad multicultural de Colombia con una explosión de color, ritmo y creatividad que pocos eventos en el mundo pueden igualar. Desde el miércoles hasta el martes siguiente al Miércoles de Ceniza, la ciudad se transforma en un escenario a cielo abierto: la Batalla de Flores inaugura el carnaval con carrozas monumentales y reinas ataviadas con flores naturales; el Gran Parada concentra las 300 danzas tradicionales; y la lectura del testamento de Joselito Carnaval cierra el festejo con humor y melancolía.\n\nLa cumbia —ritmo nacional de Colombia— nació en esta región: la fusión del tambor africano, la flauta de millo indígena y el acordeón europeo dio lugar a una música que hoy se baila en toda América Latina. Los Discos Fuentes, fundados en Barranquilla en 1934, son el sello discográfico más antiguo de Colombia y el responsable de grabar gran parte de la música tropical colombiana del siglo XX.\n\nLa gastronomía barranquillera es costeña en toda su expresión: el arroz de lisa, el mote de queso, los carimañolas (buñuelos de yuca con carne), el bollo de yuca y los refrescos de corozo son platos que llenan las calles del barrio El Prado durante las noches de carnaval.",
    eventos: [
      { nombre: "Carnaval de Barranquilla (UNESCO)", fecha: "Febrero 2025", icono: "🎭" },
      { nombre: "Festival Folclórico del Caribe", fecha: "Julio 2025", icono: "🪘" },
      { nombre: "Barranquijazz Festival", fecha: "Octubre 2025", icono: "🎷" },
    ],
    sitios: [
      { nombre: "Puerto Colombia y Muelle de Salgar", foto: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=300&h=200&fit=crop&auto=format", mapsQuery: "Puerto+Colombia+Barranquilla" },
      { nombre: "Barrio El Prado (Patrimonio)", foto: "https://images.unsplash.com/photo-1583531352515-8884af319dc1?w=300&h=200&fit=crop&auto=format", mapsQuery: "Barrio+El+Prado+Barranquilla" },
      { nombre: "Puente Pumarejo sobre el Magdalena", foto: "https://images.unsplash.com/photo-1778188985186-25a9d5f7d2b8?w=300&h=200&fit=crop&auto=format", mapsQuery: "Puente+Pumarejo+Barranquilla" },
    ],
    restaurantes: [
      { nombre: "La Cueva de Gabito", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina caribeña clásica", mapsQuery: "La+Cueva+Restaurante+Barranquilla" },
      { nombre: "El Patio de Hoyos", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Mariscos y cocina costeña", mapsQuery: "El+Patio+de+Hoyos+Barranquilla" },
    ],
  },
  // ── 20. MANIZALES ─────────────────────────────────────────────────────────
  {
    id: 20, name: "Manizales", region: "Caldas · Eje Cafetero", tagline: "La Ciudad de las Puertas Abiertas",
    description: "Ciudad de nieblas andinas y cafetales infinitos. La Feria de Manizales, el Cable Aéreo y el Nevado del Ruiz definen una identidad cafetera única.",
    image: "https://images.unsplash.com/photo-1515366974328-f1181eb25189?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=1200&h=500&fit=crop&auto=format",
    rating: 4.5, weather: "17°C · Nublado", mapsQuery: "Manizales+Caldas+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Manizales--Colombia/homes",
    historia: "Manizales fue fundada el 12 de octubre de 1849 por colonos antioqueños que buscaban nuevas tierras hacia el sur, en plena Colonización Antioqueña. Su posición en la cordillera Central, a 2.200 metros sobre el nivel del mar, rodeada de volcanes activos y nieblas eternas, le da un perfil de ciudad de montaña único en Colombia.\n\nEn 1985, la erupción del Volcán Nevado del Ruiz —el desastre natural más devastador de la historia de Colombia— destruyó la ciudad de Armero con una avalancha de lodo que mató a más de 23.000 personas. Aunque Manizales no fue directamente afectada, la tragedia marcó profundamente a toda la región y generó una nueva conciencia sobre la gestión del riesgo volcánico en Colombia.\n\nLa Universidad de Caldas y la Universidad Nacional sede Manizales son ejes del carácter universitario de la ciudad, que con más de 100.000 estudiantes matriculados en todas sus instituciones de educación superior es considerada una de las ciudades más universitarias de Colombia por habitante. Este perfil intelectual se refleja en una vida cultural activa y en la celebración de la Feria de Manizales, una de las más tradiciones ferias colombianas que combina cultura, toros y gastronomía en enero.",
    cultura: "La Feria de Manizales, celebrada cada enero, es el evento cultural más importante del Eje Cafetero: corridas de toros con figuras internacionales en la Plaza de Toros La Monumental (una de las más grandes de Colombia), conciertos multitudinarios, desfile de comparsas y la elección de la Reina Internacional del Café, que congrega candidatas de los países productores de café de todo el mundo.\n\nEl Cable Aéreo de Manizales, inaugurado en 1916, fue el primer sistema de cable de carga del hemisferio occidental: 72 km de cables transportaron café desde las fincas del Quindío hasta el río Magdalena durante décadas. Hoy, un cable moderno de pasajeros ofrece vistas espectaculares sobre la ciudad y los cafetales.\n\nLa Catedral de Manizales, con sus torres de 104 metros que dominan el skyline de la ciudad, es una de las construcciones religiosas más imponentes de Colombia. La Plaza de Bolívar y el barrio de Los Agustinos conservan la arquitectura antioqueña de principios del siglo XX con sus coloridas fachadas de madera tallada.",
    eventos: [
      { nombre: "Feria de Manizales (Enero)", fecha: "Enero 2026", icono: "☕" },
      { nombre: "Festival Internacional de Teatro", fecha: "Octubre 2025", icono: "🎭" },
      { nombre: "Festival del Bambuco", fecha: "Junio 2025", icono: "🎻" },
    ],
    sitios: [
      { nombre: "Nevado del Ruiz (Volcán activo)", foto: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Nevado+del+Ruiz+Manizales" },
      { nombre: "Cable Aéreo de Manizales", foto: "https://images.unsplash.com/photo-1515366974328-f1181eb25189?w=300&h=200&fit=crop&auto=format", mapsQuery: "Cable+Aereo+Manizales+Colombia" },
      { nombre: "Catedral de Manizales", foto: "https://images.unsplash.com/photo-1534943441045-1009d7cb0bb9?w=300&h=200&fit=crop&auto=format", mapsQuery: "Catedral+de+Manizales+Colombia" },
    ],
    restaurantes: [
      { nombre: "La Suiza de Manizales", foto: "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=300&h=200&fit=crop&auto=format", tipo: "Fondues y cocina de montaña", mapsQuery: "La+Suiza+Restaurante+Manizales" },
      { nombre: "El Rincón Caldense", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Cocina caldense y café de origen", mapsQuery: "El+Rincon+Caldense+Manizales" },
    ],
  },
  // ── 21. BUCARAMANGA ───────────────────────────────────────────────────────
  {
    id: 21, name: "Bucaramanga", region: "Santander", tagline: "La Ciudad Bonita del Chicamocha",
    description: "La Ciudad Bonita de Colombia, con el impresionante Cañón del Chicamocha y la tradición extrema del parapente, rafting y espeleología.",
    image: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=1200&h=500&fit=crop&auto=format",
    rating: 4.6, weather: "23°C · Templado", mapsQuery: "Bucaramanga+Santander+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Bucaramanga--Colombia/homes",
    historia: "Bucaramanga fue fundada el 22 de diciembre de 1622 sobre una meseta en la cordillera Oriental, a 959 metros sobre el nivel del mar. Su nombre proviene del vocablo indígena 'Bukaramanga', que en lengua guane significa 'ciudad de la bejuca de Mangle'. Los Guane, una de las ramas culturales de los Muiscas, fueron el pueblo originario de la región, reconocidos por sus avanzadas técnicas textiles y su manejo del oro.\n\nDurante el siglo XIX, Bucaramanga fue un activo centro republicano con una intensa vida política. El General Custodio García Rovira, uno de los próceres de la Independencia, era oriundo de la región. A finales del XIX y principios del XX, la ciudad creció gracias a la industria tabacalera y del cacao, que exportaban sus productos por el río Magdalena hacia el mundo.\n\nHoy Bucaramanga es conocida como la 'Ciudad de los Parques' por su gran número de espacios verdes por habitante, y como capital del deporte de aventura en Colombia: el Cañón del Chicamocha, el Parque Nacional del Chicamocha y las formaciones de Los Estoraques han convertido a Santander en el destino de aventura más importante del país.",
    cultura: "La cultura santandereana tiene fama de directa, trabajadora y orgullosa. El bocadillo veleño —dulce de guayaba compacto envuelto en hoja de palma— es el producto gastronómico más emblemático del departamento y se exporta a toda América Latina. Las hormigas culonas (Atta laevigata) son el snack más insólito y reconocido de Santander: fritas con sal, se consumen como maní y son un manjar local con miles de años de tradición indígena.\n\nEl rafting en el Río Fonce (categoría 3-4), el parapente en Mesa de Ruitoque con sus corrientes térmicas perfectas para volar durante horas, la espeleología en las cuevas de la Vaca y el canopy en Chicamocha hacen de la región santandereana la capital del turismo de aventura de Colombia.\n\nLa Catedral del Sagrado Corazón de Jesús, inaugurada en 2013, es la catedral más grande de Colombia y domina el skyline de Bucaramanga con sus torres de cristal. El Parque Nacional del Chicamocha ofrece un teleférico espectacular que cruza el cañón a 300 metros de profundidad, con vistas que dejan sin aliento sobre el río Chicamocha serpenteando entre montañas rojizas.",
    eventos: [
      { nombre: "Feria de Bucaramanga (Junio)", fecha: "Junio 2025", icono: "🎡" },
      { nombre: "Festival Internacional de Cometas", fecha: "Agosto 2025", icono: "🪁" },
      { nombre: "Colombia Extreme Sports Fest", fecha: "Septiembre 2025", icono: "🪂" },
    ],
    sitios: [
      { nombre: "Cañón del Chicamocha (Teleférico)", foto: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=300&h=200&fit=crop&auto=format", mapsQuery: "Canon+del+Chicamocha+Santander" },
      { nombre: "Los Estoraques (Parque Natural)", foto: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Los+Estoraques+Norte+de+Santander" },
      { nombre: "Mesa de Ruitoque (Parapente)", foto: "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?w=300&h=200&fit=crop&auto=format", mapsQuery: "Mesa+de+Ruitoque+Bucaramanga" },
    ],
    restaurantes: [
      { nombre: "La Carambola", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina santandereana con hormiga culona", mapsQuery: "La+Carambola+Bucaramanga" },
      { nombre: "El Viejo Chiflas", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Carne a la llanera y bocadillo", mapsQuery: "El+Viejo+Chiflas+Bucaramanga" },
    ],
  },
  // ── 22. PASTO ─────────────────────────────────────────────────────────────
  {
    id: 22, name: "Pasto", region: "Nariño", tagline: "La Ciudad del Carnaval de Negros y Blancos",
    description: "A los pies del Volcán Galeras, Pasto celebra el Carnaval más colorido de Colombia y es puerta al Santuario de Las Lajas y la Laguna de la Cocha.",
    image: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=1200&h=500&fit=crop&auto=format",
    rating: 4.5, weather: "14°C · Frío", mapsQuery: "Pasto+Nariño+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Pasto--Colombia/homes",
    historia: "San Juan de Pasto fue fundada en 1537 por el conquistador Lorenzo de Aldana, en los territorios del pueblo Quillacingas, al pie del Volcán Galeras. Su historia colonial estuvo marcada por su lealtad a la Corona española durante las guerras de Independencia, lo que le valió el apodo de 'la Ciudad Realista' y cruentas batallas con las fuerzas patriotas. Solo en 1822, tras años de resistencia, Pasto fue definitivamente incorporada a la Gran Colombia.\n\nEl Volcán Galeras, cuya última gran erupción ocurrió en 2010, es uno de los volcanes más activos de Colombia y uno de los de mayor riesgo en América del Sur: la ciudad de Pasto está ubicada en su ladera oriental a apenas 9 km del cráter, lo que la convierte en la ciudad más cercana a un volcán activo de toda Colombia.\n\nEl Carnaval de Blancos y Negros, celebrado cada enero en Pasto, fue declarado Patrimonio Cultural Inmaterial de la Humanidad por la UNESCO en 2009. Este carnaval, uno de los más coloridos de Colombia, celebra la diversidad étnica con carrozas monumentales que pesan hasta 50 toneladas y son construidas durante todo el año por artesanos locales.",
    cultura: "El Carnaval de Negros y Blancos es la expresión cultural más impactante de Nariño: el 5 de enero (Día de los Negros), los pastusos se pintan la cara de negro en referencia a la abolición de la esclavitud; el 6 de enero (Día de los Blancos), se arrojan talco y espuma blanca en celebración de la igualdad. Entre medias, el 3 y 4 de enero, desfiles de años viejos (muñecos gigantes que critican la actualidad) y juego de Coches (vehículos artesanales decorados) llenan las calles.\n\nEl barniz de Pasto —una técnica artesanal que usa resina de la planta mopa-mopa para decorar objetos de madera con colores brillantes— es una expresión artesanal única de Nariño declarada Patrimonio Cultural Inmaterial de Colombia. Los objetos decorados con barniz de Pasto (cajas, baúles, instrumentos musicales) son coleccionados en museos de arte popular de todo el mundo.\n\nEl Santuario de Las Lajas, ubicado a 77 km de Pasto cerca de Ipiales, es una de las basílicas más impresionantes de América: construida entre 1916 y 1948 sobre un puente natural de piedra en un cañón del río Guáitara, su imagen de la Virgen fue supuestamente aparecida milagrosamente en la roca. Es uno de los lugares de peregrinación más visitados de Colombia.",
    eventos: [
      { nombre: "Carnaval de Negros y Blancos (UNESCO)", fecha: "Enero 2026", icono: "🎭" },
      { nombre: "Festival Folclórico Nariñense", fecha: "Septiembre 2025", icono: "🎶" },
      { nombre: "Peregrinación Santuario Las Lajas", fecha: "Todo el año", icono: "⛪" },
    ],
    sitios: [
      { nombre: "Santuario de Nuestra Señora de Las Lajas", foto: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Santuario+Las+Lajas+Ipiales+Colombia" },
      { nombre: "Laguna de la Cocha", foto: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=300&h=200&fit=crop&auto=format", mapsQuery: "Laguna+de+la+Cocha+Pasto" },
      { nombre: "Volcán Galeras", foto: "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?w=300&h=200&fit=crop&auto=format", mapsQuery: "Volcan+Galeras+Pasto+Colombia" },
    ],
    restaurantes: [
      { nombre: "El Güilo Restaurante", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina nariñense andina", mapsQuery: "El+Guilo+Restaurante+Pasto" },
      { nombre: "La Merced Colonial", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Gastronomía típica de Nariño", mapsQuery: "La+Merced+Colonial+Pasto" },
    ],
  },
  // ── 23. VALLEDUPAR ────────────────────────────────────────────────────────
  {
    id: 23, name: "Valledupar", region: "Cesar · Costa Caribe", tagline: "La Cuna del Vallenato",
    description: "La capital del vallenato, la música más colombiana del mundo. El Festival de la Leyenda Vallenata UNESCO reúne cada abril a los mejores acordeoneros del planeta.",
    image: "https://images.unsplash.com/photo-1559827291-72ee739d0d9a?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1618426018046-77564bc4b0eb?w=1200&h=500&fit=crop&auto=format",
    rating: 4.6, weather: "33°C · Caluroso", mapsQuery: "Valledupar+Cesar+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Valledupar--Colombia/homes",
    historia: "Valledupar fue fundada el 6 de enero de 1550 por Hernando de Santana en el valle del río Guatapurí, al pie de la Sierra Nevada de Santa Marta. Su nombre proviene de la unión de 'Valle' y 'Upar', el cacique de los pueblo indígenas Chimilas que habitaban la región. Durante la Colonia fue un punto de intercambio entre los pueblos de la Sierra Nevada y las redes comerciales caribeñas.\n\nLa música vallenata nació en estas tierras a finales del siglo XIX como síntesis de tres herencias culturales: la caja (tambor africano), la guacharaca (rasqueta indígena) y el acordeón (instrumento europeo traído por colonos alemanes). Los primeros juglares vallenatos —Francisco el Hombre, Abel Antonio Villa— viajaban a pie por los caminos del Caribe compartiendo noticias y amores a través de la música.\n\nEn 2015, la UNESCO declaró el vallenato Patrimonio Cultural Inmaterial de la Humanidad, reconociendo su valor como forma de expresión cultural única. El Festival de la Leyenda Vallenata, creado por el escritor Gabriel García Márquez y el periodista Alfonso López Michelsen en 1968, se celebra cada abril y selecciona al mejor rey vallenato del año en un concurso que sigue siendo el certamen musical más importante de Colombia.",
    cultura: "El vallenato tiene cuatro aires: el paseo (alegre, narrador), el son (el más andino), el merengue (rápido y festivo) y la puya (frenética, la más compleja). Cada uno tiene su propio ritmo, carácter y técnica en el acordeón. Los grandes compositores —Carlos Vives, Carlos Vidal, Diomedes Díaz, Silvio Brito, Escalona— elevaron el vallenato a las listas internacionales y lo sacaron del folclore para convertirlo en fenómeno de masas.\n\nEl río Guatapurí es el corazón social de Valledupar: en sus orillas de aguas cristalinas y frías que bajan de la Sierra Nevada, los valleparenses se reúnen cada tarde para refrescarse del calor intenso en lo que se conoce como 'bañarse en el Guatapurí'. Este ritual social convierte las riberas del río en el balneario natural más popular de la ciudad.\n\nLa Plaza Alfonso López, epicentro del Festival Vallenato y de la vida cotidiana de Valledupar, es el corazón cívico y cultural de la ciudad. El Parque de la Leyenda Vallenata, con sus esculturas de los cuatro aires musicales, y la estatua de Francisco el Hombre son los íconos visuales de una ciudad que vive para su música.",
    eventos: [
      { nombre: "Festival de la Leyenda Vallenata (UNESCO)", fecha: "Abril 2025", icono: "🪗" },
      { nombre: "Carnaval de la Leyenda Vallenata", fecha: "Febrero 2025", icono: "🎉" },
      { nombre: "Festival de Danzas y Leyenda", fecha: "Agosto 2025", icono: "💃" },
    ],
    sitios: [
      { nombre: "Plaza Alfonso López (Festival Vallenata)", foto: "https://images.unsplash.com/photo-1559827291-72ee739d0d9a?w=300&h=200&fit=crop&auto=format", mapsQuery: "Plaza+Alfonso+Lopez+Valledupar" },
      { nombre: "Río Guatapurí (Balneario)", foto: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=300&h=200&fit=crop&auto=format", mapsQuery: "Rio+Guatapuri+Valledupar" },
      { nombre: "Parque de la Leyenda Vallenata", foto: "https://images.unsplash.com/photo-1618426018046-77564bc4b0eb?w=300&h=200&fit=crop&auto=format", mapsQuery: "Parque+Leyenda+Vallenata+Valledupar" },
    ],
    restaurantes: [
      { nombre: "El Parque Restaurante", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Cocina valduparense tradicional", mapsQuery: "El+Parque+Restaurante+Valledupar" },
      { nombre: "La Candela", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Asado y gastronomía del Cesar", mapsQuery: "La+Candela+Valledupar+Colombia" },
    ],
  },
  // ── 24. PEREIRA ───────────────────────────────────────────────────────────
  {
    id: 24, name: "Pereira", region: "Risaralda · Eje Cafetero", tagline: "La Ciudad sin Puertas",
    description: "Capital del Eje Cafetero y epicentro de la cultura cafetera. Hospitalidad sin límites, termales de montaña y el corazón agrícola de Colombia.",
    image: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1515366974328-f1181eb25189?w=1200&h=500&fit=crop&auto=format",
    rating: 4.5, weather: "21°C · Variable", mapsQuery: "Pereira+Risaralda+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Pereira--Colombia/homes",
    historia: "Pereira fue fundada el 30 de agosto de 1863 por el padre Remigio Antonio Cañarte y un grupo de familias antioqueñas que colonizaron las llanuras del río Otún y la quebrada Egoyá. Fue la última ciudad grande fundada en Colombia durante el proceso de la Colonización Antioqueña. El apodo de 'La Ciudad sin Puertas' hace referencia a la proverbial hospitalidad de los pereiranos, que supuestamente nunca cerraban sus puertas al visitante.\n\nEl terremoto de 1999 sacudió el Eje Cafetero con una magnitud de 6.2 y causó más de 1.000 muertos y la destrucción de gran parte de la ciudad de Armenia, mientras Pereira sufrió daños importantes. La reconstrucción posterior fue un modelo de resiliencia comunitaria y planificación urbana que transformó a la región en un referente de gestión del riesgo en Colombia.\n\nHoy, Pereira es una ciudad universitaria con más de 80.000 estudiantes y es reconocida como una de las ciudades más emprendedoras de Colombia. Su posición geográfica central en el Triángulo de Oro (Bogotá-Medellín-Cali) la convierte en un nodo logístico estratégico y el aeropuerto internacional Matecaña la conecta con las principales ciudades del país.",
    cultura: "Pereira es la capital informal del Eje Cafetero: desde aquí parten las rutas a los mejores fincas cafeteras, a los Termales de Santa Rosa de Cabal, al Parque Nacional Natural Los Nevados y al vecino municipio de Salento. El Parque Ukumarí, uno de los mejores zoológicos de Colombia con más de 1.200 animales, es un destino familiar imprescindible.\n\nLa Plaza de Bolívar de Pereira alberga una escultura ecuestre de Bolívar desnudo —obra del maestro Rodrigo Arenas Betancurt— que causó polémica cuando fue inaugurada en 1963 pero es hoy uno de los símbolos más queridos de la ciudad. El Viaducto César Gaviria Trujillo, inaugurado en 1997, es el puente colgante más importante del Eje Cafetero y conecta a Pereira con Dosquebradas.\n\nLos Termales de Santa Rosa de Cabal, a 30 minutos de Pereira, son aguas termales naturales que brotan de las entrañas de los Andes a más de 80°C y se enfrían en piscinas naturales entre cascadas de selva nublada. Combinados con el café de la región, las fincas de guadua y los paisajes andinos de neblina, hacen del Eje Cafetero uno de los destinos más completos de Colombia.",
    eventos: [
      { nombre: "Feria de Pereira (Agosto)", fecha: "Agosto 2025", icono: "☕" },
      { nombre: "Festival Pereira Ciudad Bonita", fecha: "Octubre 2025", icono: "🌸" },
      { nombre: "Expo Café Eje Cafetero", fecha: "Mayo 2025", icono: "🌱" },
    ],
    sitios: [
      { nombre: "Termales de Santa Rosa de Cabal", foto: "https://images.unsplash.com/photo-1580163490045-183de0c98b25?w=300&h=200&fit=crop&auto=format", mapsQuery: "Termales+Santa+Rosa+de+Cabal" },
      { nombre: "Parque Ukumarí (Zoológico)", foto: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Parque+Ukumari+Pereira" },
      { nombre: "Viaducto César Gaviria y Centro", foto: "https://images.unsplash.com/photo-1515366974328-f1181eb25189?w=300&h=200&fit=crop&auto=format", mapsQuery: "Viaducto+Pereira+Risaralda" },
    ],
    restaurantes: [
      { nombre: "El Portal del Quindío", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Gastronomía cafetera y regional", mapsQuery: "El+Portal+del+Quindio+Pereira" },
      { nombre: "Café & Restaurante El Origen", foto: "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=300&h=200&fit=crop&auto=format", tipo: "Café de origen y cocina local", mapsQuery: "El+Origen+Cafe+Pereira+Colombia" },
    ],
  },
  // ── 25. PUERTO NARIÑO ─────────────────────────────────────────────────────
  {
    id: 25, name: "Puerto Nariño", region: "Amazonas", tagline: "El Remanso Ecológico del Amazonas",
    description: "El único municipio colombiano sin automóviles ni motos. Un paraíso ecológico en la ribera del Amazonas con comunidades Tikuna y delfines rosados.",
    image: "https://images.unsplash.com/photo-1551632811-561732d1e306?w=600&h=400&fit=crop&auto=format",
    heroImage: "https://images.unsplash.com/photo-1516026672322-bc52d61a55d5?w=1200&h=500&fit=crop&auto=format",
    rating: 4.8, weather: "31°C · Húmedo", mapsQuery: "Puerto+Nariño+Amazonas+Colombia",
    airbnbUrl: "https://www.airbnb.com/s/Puerto-Nari%C3%B1o--Colombia/homes",
    historia: "Puerto Nariño fue fundado en 1961 por misioneros capuchinos como misión evangelizadora entre las comunidades indígenas Tikuna del Amazonas, y debe su nombre al prócer Antonio Nariño. Con apenas 8.000 habitantes distribuidos en el casco urbano y 23 comunidades indígenas ribereñas, es el municipio más pequeño del departamento de Amazonas y el más alejado de cualquier ciudad colombiana.\n\nSu característica más singular es la prohibición total de vehículos de motor dentro del casco urbano: no hay automóviles, no hay motos, no hay buses. Los habitantes se desplazan a pie, en bicicleta o en chalupa (lancha de motor) por el río. Esta restricción, adoptada por consenso comunitario, fue uno de los primeros ejemplos de movilidad sostenible en Colombia y convierte a Puerto Nariño en el municipio más limpio y silencioso del país.\n\nEl acceso solo es posible en chalupa desde Leticia (3-4 horas río arriba por el Amazonas) o en avioneta, lo que garantiza que el turismo que llega es selectivo, interesado en la naturaleza y respetuoso con el entorno. El municipio ha ganado premios nacionales por gestión ambiental y se perfila como un modelo de turismo responsable para toda Latinoamérica.",
    cultura: "Las comunidades Tikuna que rodean Puerto Nariño son guardianes de saberes ancestrales que incluyen el uso de más de 500 plantas medicinales de la selva amazónica, técnicas de pesca artesanal con trampas y cerbatanas, cerámica pintada con motivos de su cosmología y la elaboración de tintes naturales a partir de frutos, arcillas y cortezas. Visitar sus comunidades con guías locales certificados es una experiencia de inmersión cultural sin parangón.\n\nEl Lago Tarapoto, a pocos minutos de Puerto Nariño, es el hábitat del boto (delfín rosado de río) y de la victoria regia, el nenúfar más grande del mundo con hojas que pueden superar los 3 metros de diámetro. Los avistamientos de delfines rosados y grises en el amanecer amazónico son una de las experiencias más mágicas disponibles para el viajero en Colombia.\n\nLa gastronomía de Puerto Nariño es profundamente amazónica: el patarasca (pez envuelto en hoja de bijao y asado al fuego), el tucupí (salsa de yuca brava fermentada), el casabe de yuca (pan indígena) y las frutas amazónicas —arazá, copoazú, camu camu— son alimentos que no se encuentran en ningún otro lugar de Colombia con la misma autenticidad.",
    eventos: [
      { nombre: "Festival de la Confraternidad Amazónica", fecha: "Julio 2025", icono: "🐬" },
      { nombre: "Celebraciones Tikuna (La Pelazón)", fecha: "Variable", icono: "🪶" },
      { nombre: "Temporada Alta Delfines Rosados", fecha: "Julio-Noviembre 2025", icono: "🌿" },
    ],
    sitios: [
      { nombre: "Lago Tarapoto (Delfines Rosados)", foto: "https://images.unsplash.com/photo-1551632811-561732d1e306?w=300&h=200&fit=crop&auto=format", mapsQuery: "Lago+Tarapoto+Puerto+Narino" },
      { nombre: "Comunidades Tikuna", foto: "https://images.unsplash.com/photo-1516026672322-bc52d61a55d5?w=300&h=200&fit=crop&auto=format", mapsQuery: "Comunidades+Tikuna+Puerto+Narino" },
      { nombre: "Mirador Natem y Ribera del Amazonas", foto: "https://images.unsplash.com/photo-1448375240586-882707db888b?w=300&h=200&fit=crop&auto=format", mapsQuery: "Mirador+Natem+Puerto+Narino+Amazonas" },
    ],
    restaurantes: [
      { nombre: "Restaurante Comunitario Tikuna", foto: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300&h=200&fit=crop&auto=format", tipo: "Cocina tradicional amazónica indígena", mapsQuery: "Restaurante+Puerto+Narino+Amazonas" },
      { nombre: "Malecón del Amazonas Comedores", foto: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300&h=200&fit=crop&auto=format", tipo: "Patarasca y frutos amazónicos", mapsQuery: "Malecon+Puerto+Narino+Colombia" },
    ],
  },
];

type TabKey = "historia" | "cultura" | "eventos" | "sitios" | "restaurantes";
type Screen = "splash" | "login" | "main";
type UserType = { name: string; email: string; token: string };

// ─── ROOT ─────────────────────────────────────────────────────────────────────
export default function App() {
  const [screen, setScreen] = useState<Screen>("login");
  const [user, setUser] = useState<UserType | null>(null);
  const [lang, setLang] = useState("ES");

  const t = (k: string) => TR[lang]?.[k] ?? TR.ES[k] ?? k;

  const handleLogin = (u: UserType) => {
    setUser(u);
    setScreen("splash");
    setTimeout(() => setScreen("main"), 2800);
  };

  const handleLogout = () => { setUser(null); setScreen("login"); };

  if (screen === "login") return <LoginScreen onLogin={handleLogin} t={t} />;
  if (screen === "splash") return <SplashScreen user={user!} />;
  return <MainScreen user={user!} onLogout={handleLogout} t={t} lang={lang} setLang={setLang} />;
}

// ─── SPLASH SCREEN ────────────────────────────────────────────────────────────
function SplashScreen({ user }: { user: UserType }) {
  const [phase, setPhase] = useState(0);
  useEffect(() => {
    const t1 = setTimeout(() => setPhase(1), 150);
    const t2 = setTimeout(() => setPhase(2), 900);
    const t3 = setTimeout(() => setPhase(3), 1600);
    return () => { clearTimeout(t1); clearTimeout(t2); clearTimeout(t3); };
  }, []);

  return (
    <div className="fixed inset-0 flex flex-col items-center justify-center overflow-hidden" style={{ background: C.navy, fontFamily: "'Poppins', sans-serif" }}>
      {[600, 420, 240].map((size, i) => (
        <div key={size} className="absolute rounded-full" style={{
          width: size, height: size,
          border: `1px solid rgba(244,211,94,${0.1 + i * 0.06})`,
          transition: `transform ${2 - i * 0.2}s ease ${i * 0.15}s, opacity ${2 - i * 0.2}s ease ${i * 0.15}s`,
          transform: phase >= 1 ? "scale(1)" : `scale(${0.2 + i * 0.1})`,
          opacity: phase >= 1 ? 1 : 0,
        }} />
      ))}

      <div style={{ transition: "transform 0.8s cubic-bezier(0.34,1.56,0.64,1), opacity 0.8s ease", transform: phase >= 1 ? "scale(1) translateY(0)" : "scale(0.2) translateY(40px)", opacity: phase >= 1 ? 1 : 0 }}>
        <ImageWithFallback src={colombiaLogo} alt="Ecos Colombia" style={{ width: 200, height: 200, objectFit: "contain", filter: "drop-shadow(0 8px 32px rgba(244,211,94,0.5))" }} />
      </div>

      <div style={{ marginTop: 20, transition: "opacity 0.7s ease 0.3s, transform 0.7s ease 0.3s", opacity: phase >= 2 ? 1 : 0, transform: phase >= 2 ? "translateY(0)" : "translateY(16px)" }}>
        <p className="text-white text-3xl font-bold tracking-tight text-center" style={{ fontWeight: 700 }}>
          Ecos <span style={{ color: C.yellow }}>Colombia</span>
        </p>
        <p className="text-center text-sm mt-1" style={{ color: "rgba(255,255,255,0.5)", fontWeight: 300 }}>Turismo colombiano</p>
      </div>

      <div style={{ marginTop: 18, transition: "opacity 0.6s ease 0.2s, transform 0.6s ease 0.2s", opacity: phase >= 3 ? 1 : 0, transform: phase >= 3 ? "translateY(0)" : "translateY(12px)" }}>
        <div className="flex items-center gap-2 px-5 py-2.5 rounded-full" style={{ background: "rgba(244,211,94,0.15)", border: "1px solid rgba(244,211,94,0.3)" }}>
          <Check size={14} style={{ color: C.yellow }} />
          <span className="text-sm font-medium" style={{ color: C.yellow }}>¡Bienvenido, {user.name.split(" ")[0]}!</span>
        </div>
      </div>

      <div className="absolute bottom-12 left-1/2 -translate-x-1/2" style={{ width: 160 }}>
        <div className="h-0.5 rounded-full overflow-hidden" style={{ background: "rgba(255,255,255,0.12)" }}>
          <div className="h-full rounded-full" style={{ background: C.yellow, transition: "width 2.4s linear", width: phase >= 1 ? "100%" : "0%" }} />
        </div>
      </div>
    </div>
  );
}

// ─── LOGIN SCREEN ─────────────────────────────────────────────────────────────
function LoginScreen({ onLogin, t }: { onLogin: (u: UserType) => void; t: (k: string) => string }) {
  const [email, setEmail] = useState(""); const [password, setPassword] = useState("");
  const [showPass, setShowPass] = useState(false); const [emailF, setEmailF] = useState(false);
  const [passF, setPassF] = useState(false); const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); setError("");
    if (!email || !password) { setError("Por favor completa todos los campos."); return; }
    setLoading(true);
    try {
      const token = await login(email, password);
      const raw = email.split("@")[0].replace(/[._]/g, " ");
      onLogin({ name: raw.charAt(0).toUpperCase() + raw.slice(1), email, token });
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "No fue posible iniciar sesión.");
    } finally {
      setLoading(false);
    }
  };
  const handleGoogle = () => { setError("El inicio de sesión con Google aún no está conectado al backend."); };

  return (
    <div className="min-h-screen flex" style={{ fontFamily: "'Poppins', sans-serif", background: C.bg }}>
      <div className="hidden lg:flex lg:w-1/2 relative flex-col overflow-hidden">
        <img src="https://images.unsplash.com/photo-1768407683731-db676a1ddcd9?w=1200&h=1600&fit=crop&auto=format" alt="Colombia" className="absolute inset-0 w-full h-full object-cover" />
        <div className="absolute inset-0" style={{ background: "rgba(36,60,143,0.58)" }} />
        <div className="absolute top-0 left-0 right-0 h-1" style={{ background: C.yellow }} />
        <div className="relative z-10 flex flex-col justify-between h-full p-12">
          <div className="flex items-center gap-3">
            <ImageWithFallback src={colombiaLogo} alt="Logo" style={{ width: 44, height: 44, objectFit: "contain" }} />
            <span className="text-white text-2xl font-bold" style={{ fontWeight: 700 }}>Ecos <span style={{ color: C.yellow }}>Colombia</span></span>
          </div>
          <div className="flex flex-col gap-6">
            <div className="inline-flex items-center gap-2 self-start px-4 py-1.5 rounded-full text-xs font-semibold tracking-wider uppercase" style={{ background: "rgba(244,211,94,0.18)", color: C.yellow, border: "1px solid rgba(244,211,94,0.4)" }}>🇨🇴 Turismo colombiano</div>
            <h1 className="text-white leading-tight" style={{ fontSize: "clamp(2rem,3.5vw,3rem)", fontWeight: 700, letterSpacing: "-0.02em" }}>Explora cultura,<br />historia y destinos<br /><span style={{ color: C.yellow }}>increíbles</span> del país.</h1>
            <p className="text-white max-w-sm" style={{ fontWeight: 300, opacity: 0.85, fontSize: "0.95rem", lineHeight: 1.7 }}>Más de 1.141 municipios, 5 regiones únicas y paisajes que te dejarán sin palabras.</p>
          </div>
          <div className="flex gap-3 flex-wrap">
            {["Cartagena", "Medellín", "San Andrés", "Caño Cristales", "Salento"].map((city) => (
              <div key={city} className="flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-medium" style={{ background: "rgba(255,255,255,0.14)", color: "rgba(255,255,255,0.9)", border: "1px solid rgba(255,255,255,0.2)" }}><MapPin size={10} /> {city}</div>
            ))}
          </div>
        </div>
      </div>

      <div className="w-full lg:w-1/2 flex items-center justify-center px-6 py-12 relative">
        <div className="w-full max-w-md relative z-10">
          <div className="flex items-center gap-2 mb-10 lg:hidden">
            <ImageWithFallback src={colombiaLogo} alt="Logo" style={{ width: 32, height: 32, objectFit: "contain" }} />
            <span className="font-bold text-xl" style={{ color: C.navy }}>Ecos <span style={{ color: C.red }}>Colombia</span></span>
          </div>
          <div className="bg-white rounded-2xl px-8 py-10" style={{ boxShadow: "0 4px 6px rgba(36,60,143,0.04), 0 20px 60px rgba(36,60,143,0.10)" }}>
            <div className="mb-8">
              <h2 className="font-bold mb-2" style={{ fontSize: "2rem", color: C.navy, fontWeight: 700 }}>{t("welcome")}</h2>
              <p className="text-sm" style={{ color: "#9b9baf" }}>{t("signInContinue")}</p>
            </div>
            {error && <div className="mb-4 px-4 py-3 rounded-xl text-sm" style={{ background: "rgba(238,66,102,0.08)", color: C.red, border: "1px solid rgba(238,66,102,0.2)" }}>{error}</div>}
            <form onSubmit={handleSubmit} className="flex flex-col gap-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold" style={{ color: C.navy }}>{t("email")}</label>
                <div className="flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all duration-200" style={{ border: `1.5px solid ${emailF ? C.navy : "rgba(36,60,143,0.18)"}`, background: emailF ? "rgba(36,60,143,0.025)" : "#fafafa", boxShadow: emailF ? "0 0 0 3px rgba(36,60,143,0.08)" : "none" }}>
                  <Mail size={16} style={{ color: emailF ? C.navy : "#b0b0c4", flexShrink: 0 }} />
                  <input type="email" placeholder="tucorreo@ejemplo.com" value={email} onChange={(e) => setEmail(e.target.value)} onFocus={() => setEmailF(true)} onBlur={() => setEmailF(false)} className="flex-1 bg-transparent outline-none text-sm" style={{ color: "#2a2a3e", fontFamily: "'Poppins', sans-serif" }} />
                </div>
              </div>
              <div className="flex flex-col gap-1.5">
                <div className="flex items-center justify-between">
                  <label className="text-xs font-semibold" style={{ color: C.navy }}>{t("password")}</label>
                  <a href="#" className="text-xs font-semibold hover:opacity-70 transition-opacity" style={{ color: C.red }}>{t("forgotPassword")}</a>
                </div>
                <div className="flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all duration-200" style={{ border: `1.5px solid ${passF ? C.navy : "rgba(36,60,143,0.18)"}`, background: passF ? "rgba(36,60,143,0.025)" : "#fafafa", boxShadow: passF ? "0 0 0 3px rgba(36,60,143,0.08)" : "none" }}>
                  <Lock size={16} style={{ color: passF ? C.navy : "#b0b0c4", flexShrink: 0 }} />
                  <input type={showPass ? "text" : "password"} placeholder="••••••••" value={password} onChange={(e) => setPassword(e.target.value)} onFocus={() => setPassF(true)} onBlur={() => setPassF(false)} className="flex-1 bg-transparent outline-none text-sm" style={{ color: "#2a2a3e", fontFamily: "'Poppins', sans-serif" }} />
                  <button type="button" onClick={() => setShowPass(!showPass)} className="hover:opacity-60 transition-opacity flex-shrink-0" style={{ color: "#b0b0c4" }}>{showPass ? <EyeOff size={16} /> : <Eye size={16} />}</button>
                </div>
              </div>
              <button type="submit" disabled={loading} className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm transition-all duration-200 active:scale-95 mt-1" style={{ background: loading ? "rgba(238,66,102,0.7)" : C.red, fontWeight: 600, boxShadow: "0 4px 20px rgba(238,66,102,0.30)", cursor: loading ? "not-allowed" : "pointer" }}>
                {loading ? (<><span className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" />{t("signingIn")}</>) : (<>{t("signIn")} <ChevronRight size={16} /></>)}
              </button>
              <div className="flex items-center gap-3 my-1">
                <div className="flex-1 h-px" style={{ background: "rgba(36,60,143,0.10)" }} />
                <span className="text-xs" style={{ color: "#b0b0c4" }}>{t("orContinue")}</span>
                <div className="flex-1 h-px" style={{ background: "rgba(36,60,143,0.10)" }} />
              </div>
              <button type="button" onClick={handleGoogle} disabled={loading} className="w-full flex items-center justify-center gap-3 py-3.5 rounded-xl text-sm transition-all duration-200 hover:bg-gray-50 active:scale-95" style={{ border: "1.5px solid rgba(36,60,143,0.18)", color: "#3a3a52", fontWeight: 600, background: C.white }}>
                <svg width="18" height="18" viewBox="0 0 24 24"><path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4" /><path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853" /><path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05" /><path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335" /></svg>
                {t("continueGoogle")}
              </button>
            </form>
            <p className="text-center text-sm mt-8" style={{ color: "#9b9baf" }}>
              {t("noAccount")}{" "}<a href="#" className="font-semibold hover:opacity-70 transition-opacity" style={{ color: C.navy, fontWeight: 600 }}>{t("register")}</a>
            </p>
          </div>
          <p className="text-center text-xs mt-6" style={{ color: "#c0c0d0", fontWeight: 300 }}>
            Al iniciar sesión, aceptas nuestros <a href="#" style={{ color: "#9b9baf" }}>{t("terms")}</a> y <a href="#" style={{ color: "#9b9baf" }}>{t("privacy")}</a>.
          </p>
        </div>
      </div>
    </div>
  );
}

// ─── MAIN SCREEN ──────────────────────────────────────────────────────────────
function MainScreen({ user, onLogout, t, lang, setLang }: { user: UserType; onLogout: () => void; t: (k: string) => string; lang: string; setLang: (l: string) => void }) {
  const [modalDest, setModalDest] = useState<typeof destinations[0] | null>(null);
  const [activeTab, setActiveTab] = useState<TabKey>("historia");
  const [menuOpen, setMenuOpen] = useState(false);
  const [userMenuOpen, setUserMenuOpen] = useState(false);
  const [activeNav, setActiveNav] = useState<string | null>(null);
  const [langOpen, setLangOpen] = useState(false);
  const [mapExpanded, setMapExpanded] = useState(false);
  const [mapMinimized, setMapMinimized] = useState(false);
  const [selectedNavItem, setSelectedNavItem] = useState<string | null>(null);
  const navRef = useRef<HTMLDivElement>(null);

  const openModal = (dest: typeof destinations[0]) => { setModalDest(dest); setActiveTab("historia"); document.body.style.overflow = "hidden"; };
  const closeModal = () => { setModalDest(null); document.body.style.overflow = ""; };
  const openMaps = (q: string) => window.open(`https://www.google.com/maps/search/?api=1&query=${q}`, "_blank", "noopener,noreferrer");
  const initials = user.name.split(" ").map((w) => w[0]).slice(0, 2).join("").toUpperCase();

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (navRef.current && !navRef.current.contains(e.target as Node)) { setActiveNav(null); setLangOpen(false); setUserMenuOpen(false); }
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, []);

  const navKeys = ["navDestinos", "navExperiencias", "navCultura", "navBlog", "navContacto"];
  const navSectionKeys = ["Destinos", "Experiencias", "Cultura", "Blog", "Contacto"];

  return (
    <div className="min-h-screen" style={{ background: C.bg, fontFamily: "'Poppins', sans-serif" }}>
      {/* ── NAVBAR ── */}
      <nav ref={navRef} className="fixed top-0 left-0 right-0 z-50" style={{ background: C.white, boxShadow: "0 2px 20px rgba(36,60,143,0.10)" }}>
        <div className="flex items-center justify-between px-8 py-3.5">
          <div className="flex items-center gap-2.5">
            <ImageWithFallback src={colombiaLogo} alt="Logo" style={{ width: 36, height: 36, objectFit: "contain" }} />
            <span className="text-xl font-bold tracking-tight" style={{ color: C.navy, fontWeight: 700 }}>Ecos <span style={{ color: C.red }}>Colombia</span></span>
          </div>

          <div className="hidden md:flex items-center gap-1">
            {navKeys.map((key, i) => {
              const section = navSectionKeys[i];
              const isActive = activeNav === section;
              return (
                <button key={key} onClick={() => { setActiveNav(isActive ? null : section); setLangOpen(false); setUserMenuOpen(false); }}
                  className="px-4 py-2 rounded-lg text-sm transition-all duration-150 flex items-center gap-1"
                  style={{ color: isActive ? C.navy : "#5a5a72", background: isActive ? "rgba(36,60,143,0.08)" : "transparent", fontWeight: isActive ? 600 : 500 }}>
                  {t(key)}
                  <ChevronRight size={12} style={{ transform: isActive ? "rotate(90deg)" : "rotate(0deg)", transition: "transform 0.2s", opacity: 0.5 }} />
                </button>
              );
            })}
          </div>

          <div className="hidden md:flex items-center gap-3">
            {/* Language */}
            <div className="relative">
              <button onClick={() => { setLangOpen(!langOpen); setActiveNav(null); setUserMenuOpen(false); }}
                className="flex items-center gap-1.5 px-3 py-2 rounded-lg text-sm transition-all hover:bg-gray-50"
                style={{ color: C.navy, border: "1.5px solid rgba(36,60,143,0.15)", fontWeight: 600 }}>
                <Globe size={14} /> {lang}
              </button>
              {langOpen && (
                <div className="absolute right-0 top-full mt-2 rounded-xl overflow-hidden py-1" style={{ background: C.white, boxShadow: "0 8px 32px rgba(36,60,143,0.18)", border: "1px solid rgba(36,60,143,0.08)", minWidth: "160px", zIndex: 60 }}>
                  {LANGUAGES.map((l) => (
                    <button key={l.code} onClick={() => { setLang(l.code); setLangOpen(false); }}
                      className="w-full flex items-center justify-between px-4 py-2.5 text-sm hover:bg-gray-50 transition-colors"
                      style={{ color: lang === l.code ? C.navy : "#5a5a72", fontWeight: lang === l.code ? 600 : 400 }}>
                      <span className="flex items-center gap-2">{l.flag} {l.label}</span>
                      {lang === l.code && <Check size={13} style={{ color: C.navy }} />}
                    </button>
                  ))}
                </div>
              )}
            </div>
            {/* User */}
            <div className="relative">
              <button onClick={() => { setUserMenuOpen(!userMenuOpen); setLangOpen(false); setActiveNav(null); }}
                className="flex items-center gap-2.5 px-3 py-2 rounded-xl transition-all duration-200 hover:bg-gray-50"
                style={{ border: "1.5px solid rgba(36,60,143,0.15)" }}>
                <div className="w-7 h-7 rounded-lg flex items-center justify-center text-white text-xs font-bold" style={{ background: C.navy }}>{initials}</div>
                <span className="text-sm font-medium" style={{ color: C.navy }}>{user.name.split(" ")[0]}</span>
              </button>
              {userMenuOpen && (
                <div className="absolute right-0 top-full mt-2 w-52 rounded-xl overflow-hidden py-1" style={{ background: C.white, boxShadow: "0 8px 32px rgba(36,60,143,0.16)", border: "1px solid rgba(36,60,143,0.08)", zIndex: 60 }}>
                  <div className="px-4 py-3 border-b" style={{ borderColor: "rgba(36,60,143,0.08)" }}>
                    <p className="text-sm font-semibold" style={{ color: C.navy }}>{user.name}</p>
                    <p className="text-xs truncate" style={{ color: "#9b9baf" }}>{user.email}</p>
                  </div>
                  <button className="w-full flex items-center gap-2.5 px-4 py-2.5 text-sm hover:bg-gray-50 transition-colors" style={{ color: "#5a5a72" }}><User size={14} /> {t("miPerfil")}</button>
                  <button onClick={() => { setUserMenuOpen(false); onLogout(); }} className="w-full flex items-center gap-2.5 px-4 py-2.5 text-sm hover:bg-red-50 transition-colors" style={{ color: C.red }}><LogOut size={14} /> {t("cerrarSesion")}</button>
                </div>
              )}
            </div>
          </div>

          <button className="md:hidden p-2 rounded-lg" style={{ color: C.navy }} onClick={() => setMenuOpen(!menuOpen)}><Menu size={22} /></button>
        </div>

        {/* Nav dropdown */}
        {activeNav && (
          <div className="border-t hidden md:block" style={{ borderColor: "rgba(36,60,143,0.08)", background: C.white }}>
            <div className="max-w-5xl mx-auto px-8 py-6 grid grid-cols-3 gap-8">
              <div className="col-span-1 pr-6 border-r" style={{ borderColor: "rgba(36,60,143,0.08)" }}>
                <h3 className="font-bold text-base mb-2" style={{ color: C.navy }}>{NAV_ITEMS_MAP[activeNav].title}</h3>
                <p className="text-xs leading-relaxed mb-4" style={{ color: "#9b9baf", fontWeight: 300 }}>{NAV_ITEMS_MAP[activeNav].desc}</p>
                <button onClick={() => setActiveNav(null)} className="flex items-center gap-1 text-xs font-semibold px-4 py-2 rounded-lg text-white" style={{ background: C.red }}>
                  Ver todo <ChevronRight size={12} />
                </button>
              </div>
              <div className="col-span-2 grid grid-cols-2 gap-1">
                {NAV_ITEMS_MAP[activeNav].items.map((item) => (
                  <button key={item.label}
                    onClick={() => {
                      setActiveNav(null);
                      setSelectedNavItem(item.label);
                      if (item.destId) {
                        const dest = destinations.find(d => d.id === item.destId);
                        if (dest) openModal(dest);
                      }
                    }}
                    className="text-left py-2.5 px-3 text-sm rounded-lg transition-all duration-150 flex items-center gap-2 hover:bg-gray-50"
                    style={{ color: selectedNavItem === item.label ? C.navy : "#3a3a52", fontWeight: selectedNavItem === item.label ? 600 : 400, background: selectedNavItem === item.label ? "rgba(36,60,143,0.06)" : "transparent" }}>
                    <MapPin size={11} style={{ color: item.destId ? C.red : "#b0b0c4", flexShrink: 0 }} />
                    {item.label}
                  </button>
                ))}
              </div>
            </div>
          </div>
        )}

        {/* Mobile menu */}
        {menuOpen && (
          <div className="border-t md:hidden" style={{ borderColor: "rgba(36,60,143,0.08)" }}>
            <div className="py-4 px-8 flex flex-col gap-2">
              {navKeys.map((key, i) => (
                <button key={key} onClick={() => { setMenuOpen(false); setActiveNav(navSectionKeys[i]); }} className="py-2.5 text-sm font-medium border-b text-left" style={{ color: C.navy, borderColor: "rgba(36,60,143,0.08)" }}>{t(key)}</button>
              ))}
              <div className="flex items-center justify-between pt-2">
                <span className="text-sm font-medium" style={{ color: C.navy }}>{user.name}</span>
                <button onClick={onLogout} className="text-xs font-semibold flex items-center gap-1" style={{ color: C.red }}><LogOut size={13} /> {t("cerrarSesion")}</button>
              </div>
            </div>
          </div>
        )}
      </nav>

      {/* ── HERO ── */}
      <section className="relative flex flex-col items-center justify-center text-center" style={{ height: 520, marginTop: 64 }}>
        <img src="https://images.unsplash.com/photo-1778188985186-25a9d5f7d2b8?w=1600&h=600&fit=crop&auto=format" alt="Paisaje colombiano" className="absolute inset-0 w-full h-full object-cover" />
        <div className="absolute inset-0" style={{ background: "rgba(36,60,143,0.58)" }} />
        <div className="absolute top-0 left-0 right-0 h-1" style={{ background: C.yellow }} />
        <div className="relative z-10 px-6 max-w-4xl mx-auto">
          <div className="inline-flex items-center gap-2 px-4 py-1.5 rounded-full mb-6 text-xs font-semibold tracking-wider uppercase" style={{ background: "rgba(244,211,94,0.22)", color: C.yellow, border: "1px solid rgba(244,211,94,0.45)" }}>
            👋 {t("hola")}, {user.name.split(" ")[0]} — {t("heroBadge")}
          </div>
          <h1 className="text-white mb-4 leading-tight" style={{ fontSize: "clamp(38px,7vw,64px)", fontWeight: 700, letterSpacing: "-0.02em" }}>{t("heroTitle")}</h1>
          <p className="text-white mb-4 max-w-xl mx-auto" style={{ fontSize: "1.1rem", fontWeight: 300, opacity: 0.9 }}>{t("heroSub")}</p>
        </div>
        <div className="absolute bottom-0 left-1/2 -translate-x-1/2 translate-y-1/2 z-20 hidden md:flex divide-x rounded-2xl overflow-hidden" style={{ background: C.white, boxShadow: "0 8px 40px rgba(36,60,143,0.18)" }}>
          {[{ v: "32", k: "departments" }, { v: "1.141", k: "municipalities" }, { v: "60+", k: "parks" }, { v: "5", k: "regions" }].map((s, i) => (
            <div key={i} className="px-8 py-4 flex flex-col items-center" style={{ borderColor: "rgba(36,60,143,0.08)" }}>
              <span className="text-2xl font-bold" style={{ color: C.navy }}>{s.v}</span>
              <span className="text-xs mt-0.5" style={{ color: "#717182", fontWeight: 500 }}>{t(s.k)}</span>
            </div>
          ))}
        </div>
      </section>

      {/* ── DESTINATIONS ── */}
      <section id="destinos" className="max-w-7xl mx-auto px-6 pb-16" style={{ paddingTop: 80 }}>
        <div className="flex flex-col md:flex-row items-start md:items-center justify-between mb-10 gap-4">
          <div>
            <p className="text-xs font-semibold uppercase tracking-widest mb-1" style={{ color: C.red }}>{t("sectionLabel")}</p>
            <h2 className="text-3xl font-bold" style={{ color: C.navy, fontWeight: 700 }}>{t("sectionTitle")}</h2>
          </div>
          <button className="flex items-center gap-2 text-sm px-5 py-2.5 rounded-xl transition-all hover:opacity-80" style={{ color: C.navy, background: "rgba(36,60,143,0.07)", fontWeight: 600 }}>
            {t("sectionBtn")} <ChevronRight size={15} />
          </button>
        </div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))", gap: 25 }}>
          {destinations.map((dest) => (
            <DestCard key={dest.id} dest={dest} onClick={() => openModal(dest)} cardBtn={t("cardBtn")} />
          ))}
        </div>
      </section>

      {/* ── FLOATING MAP ── */}
      <FloatingMap expanded={mapExpanded} minimized={mapMinimized} onToggleExpand={() => { setMapExpanded(!mapExpanded); setMapMinimized(false); }} onToggleMinimize={() => setMapMinimized(!mapMinimized)} onOpenMaps={openMaps} t={t} />

      {/* ── MODAL ── */}
      {modalDest && <DestModal dest={modalDest} activeTab={activeTab} setActiveTab={setActiveTab} onClose={closeModal} onOpenMaps={openMaps} t={t} />}
    </div>
  );
}

// ─── DESTINATION CARD ─────────────────────────────────────────────────────────
function DestCard({ dest, onClick, cardBtn }: { dest: typeof destinations[0]; onClick: () => void; cardBtn: string }) {
  return (
    <div className="group bg-white cursor-pointer overflow-hidden transition-all duration-300 hover:-translate-y-2" style={{ borderRadius: 18, boxShadow: "0 4px 20px rgba(36,60,143,0.08)" }} onClick={onClick}>
      <div className="relative overflow-hidden" style={{ height: 240 }}>
        <img src={dest.image} alt={dest.name} className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110" />
        <div className="absolute inset-0 bg-gradient-to-t from-black/30 to-transparent" />
        <div className="absolute top-4 left-4 px-3 py-1 rounded-full text-xs font-semibold" style={{ background: "rgba(255,255,255,0.92)", color: C.navy }}>{dest.region}</div>
        <div className="absolute top-4 right-4 flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold" style={{ background: C.yellow, color: C.navy }}><Star size={11} fill={C.navy} /> {dest.rating}</div>
      </div>
      <div className="p-6">
        <h3 className="text-xl font-bold mb-1" style={{ color: C.navy, fontWeight: 700 }}>{dest.name}</h3>
        <p className="text-xs font-semibold mb-3" style={{ color: C.red }}>{dest.tagline}</p>
        <p className="text-sm leading-relaxed mb-5" style={{ color: "#5a5a72", fontWeight: 300 }}>{dest.description}</p>
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-1.5 text-xs" style={{ color: "#717182" }}><span>🌤</span><span style={{ fontWeight: 500 }}>{dest.weather}</span></div>
          <button className="flex items-center gap-1.5 px-4 py-2 rounded-xl text-xs text-white transition-all hover:opacity-90 active:scale-95" style={{ background: C.red, fontWeight: 600 }}>
            {cardBtn} <ChevronRight size={13} />
          </button>
        </div>
      </div>
    </div>
  );
}

// ─── FLOATING MAP ─────────────────────────────────────────────────────────────
function FloatingMap({ expanded, minimized, onToggleExpand, onToggleMinimize, onOpenMaps, t }: {
  expanded: boolean; minimized: boolean;
  onToggleExpand: () => void; onToggleMinimize: () => void;
  onOpenMaps: (q: string) => void; t: (k: string) => string;
}) {
  const [hovered, setHovered] = useState<string | null>(null);
  const w = expanded ? 440 : 300;
  const mapH = expanded ? 400 : 258;
  const dotSize = expanded ? 10 : 8;
  const ringSize = expanded ? 18 : 14;

  return (
    <div
      className="fixed bottom-6 right-6 z-40 flex flex-col transition-all duration-300"
      style={{ width: w, borderRadius: 20, background: C.white, border: "5px solid white", boxShadow: "0 12px 48px rgba(36,60,143,0.22), 0 2px 8px rgba(0,0,0,0.08)", overflow: "hidden" }}
    >
      {/* Header */}
      <div className="flex items-center justify-between px-4 py-3 flex-shrink-0" style={{ background: C.navy }}>
        <div className="flex items-center gap-2">
          <MapPin size={14} className="text-white" />
          <span className="text-xs font-semibold text-white tracking-wide">{t("mapTitle")}</span>
        </div>
        <div className="flex items-center gap-1.5">
          <span className="text-xs mr-1" style={{ color: C.yellow, fontWeight: 500 }}>
            {ALL_MARKERS.filter(m => m.dest).length} destinos
          </span>
          <button onClick={onToggleMinimize} className="w-6 h-6 rounded flex items-center justify-center transition-all hover:bg-white/20" title={minimized ? t("mapExpand") : t("mapMinimize")}>
            <Minimize2 size={12} className="text-white" />
          </button>
          <button onClick={onToggleExpand} className="w-6 h-6 rounded flex items-center justify-center transition-all hover:bg-white/20" title={expanded ? t("mapMinimize") : t("mapExpand")}>
            <Maximize2 size={12} className="text-white" />
          </button>
        </div>
      </div>

      {!minimized && (
        <>
          {/* Map with real Colombia image + markers */}
          <div
            className="relative flex-shrink-0 transition-all duration-300"
            style={{ height: mapH, background: "#e8f4fa" }}
          >
            {/* Colombia map image */}
            <ImageWithFallback
              src={colombiaMapImg}
              alt="Mapa de Colombia"
              style={{ position: "absolute", inset: 0, width: "100%", height: "100%", objectFit: "contain", padding: "6px" }}
            />

            {/* Markers positioned as % over the image */}
            {ALL_MARKERS.map((m) => (
              <div
                key={m.name}
                className="absolute"
                style={{ left: `${m.px}%`, top: `${m.py}%`, transform: "translate(-50%, -50%)", cursor: "pointer", zIndex: 10 }}
                onMouseEnter={() => setHovered(m.name)}
                onMouseLeave={() => setHovered(null)}
                onClick={() => onOpenMaps(m.name.replace(/ /g, "+") + "+Colombia")}
              >
                {/* Pulse ring */}
                <div style={{
                  position: "absolute",
                  width: ringSize, height: ringSize,
                  borderRadius: "50%",
                  background: m.dest ? C.red : C.navy,
                  opacity: hovered === m.name ? 0.3 : 0.12,
                  top: "50%", left: "50%",
                  transform: "translate(-50%,-50%)",
                  transition: "opacity 0.2s",
                }} />
                {/* Dot */}
                <div style={{
                  width: dotSize, height: dotSize,
                  borderRadius: "50%",
                  background: m.dest ? C.red : C.navy,
                  border: "2px solid white",
                  boxShadow: "0 1px 4px rgba(0,0,0,0.25)",
                  opacity: m.dest ? 1 : 0.7,
                  position: "relative",
                  zIndex: 2,
                }} />
                {/* Inner dot for destinations */}
                {m.dest && (
                  <div style={{ position: "absolute", width: 3, height: 3, borderRadius: "50%", background: "white", top: "50%", left: "50%", transform: "translate(-50%,-50%)", zIndex: 3 }} />
                )}
                {/* Tooltip */}
                {(hovered === m.name) && (
                  <div style={{
                    position: "absolute",
                    bottom: "calc(100% + 4px)",
                    left: "50%",
                    transform: "translateX(-50%)",
                    background: C.navy,
                    color: "white",
                    fontSize: expanded ? "10px" : "9px",
                    fontWeight: 600,
                    padding: "3px 7px",
                    borderRadius: 6,
                    whiteSpace: "nowrap",
                    pointerEvents: "none",
                    boxShadow: "0 2px 8px rgba(0,0,0,0.2)",
                    fontFamily: "'Poppins', sans-serif",
                  }}>
                    {m.name}
                  </div>
                )}
                {/* Always-visible label when expanded */}
                {expanded && hovered !== m.name && (
                  <div style={{
                    position: "absolute",
                    bottom: "calc(100% + 2px)",
                    left: "50%",
                    transform: "translateX(-50%)",
                    fontSize: "8px",
                    fontWeight: 600,
                    color: C.navy,
                    whiteSpace: "nowrap",
                    pointerEvents: "none",
                    textShadow: "0 0 4px white, 0 0 4px white",
                    fontFamily: "'Poppins', sans-serif",
                  }}>
                    {m.name}
                  </div>
                )}
              </div>
            ))}
          </div>

          {/* Footer */}
          <div className="px-4 py-3 flex items-center justify-between flex-shrink-0" style={{ borderTop: "1px solid rgba(36,60,143,0.06)" }}>
            <div className="flex items-center gap-3">
              <div className="flex items-center gap-1.5">
                <div className="w-2.5 h-2.5 rounded-full" style={{ background: C.red }} />
                <span className="text-xs" style={{ color: "#717182", fontWeight: 500 }}>{t("destMarker")}</span>
              </div>
              <div className="flex items-center gap-1.5">
                <div className="w-2 h-2 rounded-full" style={{ background: C.navy }} />
                <span className="text-xs" style={{ color: "#717182", fontWeight: 500 }}>{t("cityMarker")}</span>
              </div>
            </div>
            <button onClick={() => onOpenMaps("Colombia+turismo")} className="text-xs font-semibold flex items-center gap-1 px-3 py-1.5 rounded-lg transition-all hover:opacity-80" style={{ color: C.white, background: C.navy }}>
              <Navigation size={11} /> {t("mapGoogle")}
            </button>
          </div>
        </>
      )}
    </div>
  );
}

// ─── DESTINATION MODAL ────────────────────────────────────────────────────────
function DestModal({ dest, activeTab, setActiveTab, onClose, onOpenMaps, t }: {
  dest: typeof destinations[0]; activeTab: TabKey; setActiveTab: (t: TabKey) => void;
  onClose: () => void; onOpenMaps: (q: string) => void; t: (k: string) => string;
}) {
  const tabs: { key: TabKey; label: string; icon: React.ReactNode }[] = [
    { key: "historia", label: t("tabHistoria"), icon: <BookOpen size={14} /> },
    { key: "cultura", label: t("tabCultura"), icon: <Globe size={14} /> },
    { key: "eventos", label: t("tabEventos"), icon: <Calendar size={14} /> },
    { key: "sitios", label: t("tabSitios"), icon: <Landmark size={14} /> },
    { key: "restaurantes", label: t("tabRestaurantes"), icon: <Utensils size={14} /> },
  ];

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 md:p-6" style={{ background: "rgba(10,18,50,0.72)", backdropFilter: "blur(4px)" }} onClick={(e) => { if (e.target === e.currentTarget) onClose(); }}>
      <div className="relative w-full bg-white flex flex-col overflow-hidden" style={{ maxWidth: 900, maxHeight: "90vh", borderRadius: 24, boxShadow: "0 32px 80px rgba(10,18,50,0.35)" }}>
        <button onClick={onClose} className="absolute top-4 right-4 z-10 w-9 h-9 rounded-full flex items-center justify-center transition-all hover:opacity-80" style={{ background: "rgba(10,18,50,0.4)", color: "white" }}><X size={18} /></button>

        <div className="relative flex-shrink-0" style={{ height: 280 }}>
          <img src={dest.heroImage} alt={dest.name} className="w-full h-full object-cover" />
          <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-black/10 to-transparent" />
          <div className="absolute bottom-6 left-8">
            <p className="text-xs font-semibold uppercase tracking-widest mb-1" style={{ color: C.yellow }}>{dest.region}</p>
            <h2 className="text-4xl font-bold text-white" style={{ fontWeight: 700 }}>{dest.name}</h2>
            <p className="text-white/80 text-sm mt-1" style={{ fontWeight: 300 }}>{dest.tagline}</p>
          </div>
          <div className="absolute bottom-6 right-8 flex items-center gap-1.5 px-3 py-1.5 rounded-full text-sm font-semibold" style={{ background: C.yellow, color: C.navy }}><Star size={12} fill={C.navy} /> {dest.rating} / 5.0</div>
        </div>

        <div className="flex overflow-x-auto border-b flex-shrink-0 px-6" style={{ borderColor: "rgba(36,60,143,0.1)" }}>
          {tabs.map((tab) => (
            <button key={tab.key} onClick={() => setActiveTab(tab.key)}
              className="flex items-center gap-1.5 px-4 py-3.5 text-sm whitespace-nowrap transition-all border-b-2 -mb-px"
              style={{ fontWeight: activeTab === tab.key ? 600 : 400, color: activeTab === tab.key ? C.navy : "#717182", borderColor: activeTab === tab.key ? C.red : "transparent" }}>
              {tab.icon} {tab.label}
            </button>
          ))}
        </div>

        <div className="flex-1 overflow-y-auto p-8" style={{ scrollbarWidth: "none" }}>
          {activeTab === "historia" && (
            <div>
              <h3 className="text-lg font-bold mb-4" style={{ color: C.navy }}>{t("tabHistoria")} de {dest.name}</h3>
              {dest.historia.split("\n\n").map((para, i) => (
                <p key={i} className="text-sm leading-relaxed mb-4" style={{ color: "#5a5a72", fontWeight: 300 }}>{para}</p>
              ))}
            </div>
          )}
          {activeTab === "cultura" && (
            <div>
              <h3 className="text-lg font-bold mb-4" style={{ color: C.navy }}>{t("tabCultura")} y tradiciones</h3>
              {dest.cultura.split("\n\n").map((para, i) => (
                <p key={i} className="text-sm leading-relaxed mb-4" style={{ color: "#5a5a72", fontWeight: 300 }}>{para}</p>
              ))}
            </div>
          )}
          {activeTab === "eventos" && (
            <div>
              <h3 className="text-lg font-bold mb-5" style={{ color: C.navy }}>Próximos {t("tabEventos").toLowerCase()}</h3>
              <div className="flex flex-col gap-3">
                {dest.eventos.map((ev, i) => (
                  <div key={i} className="flex items-center gap-4 p-4 rounded-xl" style={{ background: "#F7F7F7" }}>
                    <span className="text-2xl">{ev.icono}</span>
                    <div className="flex-1"><p className="text-sm font-semibold" style={{ color: C.navy }}>{ev.nombre}</p><p className="text-xs mt-0.5" style={{ color: "#717182" }}>{ev.fecha}</p></div>
                    <button className="px-3 py-1.5 rounded-lg text-xs text-white" style={{ background: C.red, fontWeight: 600 }}>Info</button>
                  </div>
                ))}
              </div>
            </div>
          )}
          {activeTab === "sitios" && (
            <div>
              <h3 className="text-lg font-bold mb-5" style={{ color: C.navy }}>{t("tabSitios")} imperdibles</h3>
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                {dest.sitios.map((s, i) => (
                  <div key={i} className="rounded-xl overflow-hidden bg-white" style={{ boxShadow: "0 2px 12px rgba(36,60,143,0.09)" }}>
                    <div style={{ height: 140, overflow: "hidden" }}><img src={s.foto} alt={s.nombre} className="w-full h-full object-cover" /></div>
                    <div className="p-4">
                      <p className="text-sm font-semibold mb-3" style={{ color: C.navy }}>{s.nombre}</p>
                      <button onClick={() => onOpenMaps(s.mapsQuery)} className="w-full flex items-center justify-center gap-1.5 py-2 rounded-lg text-xs text-white hover:opacity-90 transition-all active:scale-95" style={{ background: C.red, fontWeight: 600 }}>
                        <Navigation size={12} /> {t("howToGet")}
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
          {activeTab === "restaurantes" && (
            <div>
              <h3 className="text-lg font-bold mb-5" style={{ color: C.navy }}>{t("tabRestaurantes")} recomendados</h3>
              <div className="flex flex-col gap-4">
                {dest.restaurantes.map((r, i) => (
                  <div key={i} className="rounded-xl overflow-hidden bg-white flex flex-row" style={{ boxShadow: "0 2px 12px rgba(36,60,143,0.09)", minHeight: 130 }}>
                    {/* Image — fixed width on the left */}
                    <div style={{ width: 160, minWidth: 160, overflow: "hidden", flexShrink: 0 }}>
                      <img src={r.foto} alt={r.nombre} style={{ width: "100%", height: "100%", objectFit: "cover" }} />
                    </div>
                    {/* Content */}
                    <div className="p-5 flex flex-col justify-between flex-1">
                      <div>
                        <p className="text-sm font-bold mb-1" style={{ color: C.navy }}>{r.nombre}</p>
                        <span className="inline-block px-2.5 py-0.5 rounded-full text-xs font-medium mb-3" style={{ background: "rgba(36,60,143,0.08)", color: C.navy }}>{r.tipo}</span>
                        <p className="text-xs leading-relaxed" style={{ color: "#717182", fontWeight: 300 }}>
                          Uno de los restaurantes más recomendados de {dest.name}. Ideal para disfrutar de la gastronomía local en un ambiente auténtico.
                        </p>
                      </div>
                      <button
                        onClick={() => onOpenMaps(r.mapsQuery)}
                        className="mt-3 flex items-center justify-center gap-1.5 py-2.5 rounded-lg text-xs text-white hover:opacity-90 transition-all active:scale-95"
                        style={{ background: C.red, fontWeight: 600 }}
                      >
                        <Navigation size={13} /> {t("howToGet")}
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        <div className="flex-shrink-0 flex flex-col sm:flex-row items-center justify-between gap-4 px-8 py-5" style={{ borderTop: "1px solid rgba(36,60,143,0.08)", background: "#fafafa" }}>
          <div>
            <p className="text-sm font-semibold" style={{ color: C.navy }}>{t("readyToExplore")} {dest.name}?</p>
            <p className="text-xs" style={{ color: "#717182", fontWeight: 300 }}>{t("findAccom")}</p>
          </div>
          <a
            href={dest.airbnbUrl}
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center gap-2 px-7 py-3 rounded-xl text-sm text-white transition-all hover:opacity-90 active:scale-95 whitespace-nowrap"
            style={{ background: C.red, fontWeight: 600 }}
          >
            {t("reserveBtn")} <ChevronRight size={15} />
          </a>
        </div>
      </div>
    </div>
  );
}
