import { useState, useMemo } from 'react';
import {
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Collapse,
  Box,
  Typography,
  Divider,
  IconButton,
  useTheme,
  useMediaQuery,
} from '@mui/material';
import {
  Security as SecurityIcon,
  Person as PersonIcon,
  Settings as SettingsIcon,
  Dashboard as DashboardIcon,
  AdminPanelSettings as RoleIcon,
  Business as BusinessIcon,
  ExpandLess,
  ExpandMore,
  ChevronLeft as ChevronLeftIcon,
  ListAlt as ListIcon,
} from '@mui/icons-material';
import { useNavigate, useLocation } from 'react-router-dom';
import { usePermissions, usePermissionStore } from '../../stores/permission.store';
import { ModuleKey } from '../../types/permission.types';
import { layout } from '../../theme';

export interface SidebarProps {
  open: boolean;
  onClose: () => void;
  onToggle: () => void;
}

interface MenuSection {
  id: string;
  title: string;
  icon: React.ReactNode;
  path?: string;
  children?: MenuItem[];
  moduleKey?: string;
  operation?: string;
}

interface MenuItem {
  id: string;
  title: string;
  icon: React.ReactNode;
  path: string;
  badge?: string;
  moduleKey?: string;
  operation?: string;
}

export const Sidebar = ({ open, onClose, onToggle }: SidebarProps) => {
  const navigate = useNavigate();
  const location = useLocation();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('lg'));
  const { hasAccess } = usePermissions();
  const isInitialized = usePermissionStore(state => state.isInitialized);

  const [expandedSections, setExpandedSections] = useState<string[]>(['ecosystem', 'access-control']);

  const menuSections: MenuSection[] = [
    {
      id: 'dashboard',
      title: 'Painel Geral',
      icon: <DashboardIcon />,
      path: '/dashboard',
    },
    {
      id: 'ecosystem',
      title: 'Gestão Opamenu',
      icon: <BusinessIcon />,
      children: [
        {
          id: 'tenants',
          title: 'Clientes (Tenants)',
          icon: <BusinessIcon />,
          path: '/tenants',
          //moduleKey: ModuleKey.TENANT_MODULE,
          operation: 'SELECT',
        },
        {
          id: 'plans',
          title: 'Planos e Preços',
          icon: <ListIcon />,
          path: '/plans',
        },
      ],
    },
    {
      id: 'access-control',
      title: 'Segurança Interna',
      icon: <SecurityIcon />,
      children: [
        {
          id: 'users',
          title: 'Usuários',
          icon: <PersonIcon />,
          path: '/users',
          //moduleKey: ModuleKey.USER_MODULE,
          operation: 'SELECT',
        },
        {
          id: 'roles',
          title: 'Perfis (Roles)',
          icon: <RoleIcon />,
          path: '/roles',
          //moduleKey: ModuleKey.ROLE_MODULE,
          operation: 'SELECT',
        },
        {
          id: 'modules',
          title: 'Módulos do Sistema',
          icon: <SettingsIcon />,
          path: '/modules',
          //moduleKey: ModuleKey.MODULES,
          operation: 'SELECT',
        },
      ],
    },
    {
      id: 'settings',
      title: 'Configurações',
      icon: <SettingsIcon />,
      children: [
        {
          id: 'system',
          title: 'Global',
          icon: <SettingsIcon />,
          path: '/settings/system',
          badge: 'Em breve',
        },
        {
          id: 'profile',
          title: 'Meu Perfil',
          icon: <PersonIcon />,
          path: '/settings/profile',
          badge: 'Em breve',
        },
      ],
    },
  ];

  const { role } = usePermissionStore(); 
  const isSuperAdmin = useMemo(() => {
    const roleName = role?.name?.toUpperCase().replace(/\s+/g, '_');
    return roleName === 'SUPER_ADMIN';
  }, [role]);

  const hasPermissionForItem = (item: MenuItem | MenuSection): boolean => {
    if (isSuperAdmin) return true; // Bypass total para Super Admin
    if (!item.moduleKey) return true;
    const operation = item.operation || 'SELECT';
    return hasAccess(item.moduleKey, operation as any);
  };

  const filteredMenuSections = useMemo(() => {
    if (!isInitialized) return [];

    return menuSections
      .map(section => {
        if (section.children) {
          const filteredChildren = section.children.filter(hasPermissionForItem);
          if (filteredChildren.length === 0) return null;
          return { ...section, children: filteredChildren };
        }
        return hasPermissionForItem(section) ? section : null;
      })
      .filter((section): section is MenuSection => section !== null);
  }, [isInitialized, hasAccess]);

  const handleSectionClick = (section: MenuSection) => {
    if (section.path) {
      navigate(section.path);
      if (isMobile) onClose();
    } else if (section.children) {
      setExpandedSections(prev =>
        prev.includes(section.id)
          ? prev.filter(id => id !== section.id)
          : [...prev, section.id]
      );
    }
  };

  const handleItemClick = (item: MenuItem) => {
    if (!item.badge) {
      navigate(item.path);
      if (isMobile) onClose();
    }
  };

  const isActiveItem = (path: string) => location.pathname === path;

  const isActiveSection = (section: MenuSection) => {
    if (section.path) return isActiveItem(section.path);
    return section.children?.some(item => isActiveItem(item.path)) || false;
  };

  const drawerContent = (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box
        sx={{
          p: 2,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          minHeight: layout.headerHeight,
          borderBottom: `1px solid ${theme.palette.divider}`,
        }}
      >
        <Typography variant="h6" sx={{ fontWeight: 700, color: 'primary.main' }}>
          🛡️ Central de Comando
        </Typography>
        {!isMobile && (
          <IconButton onClick={onToggle} size="small">
            <ChevronLeftIcon />
          </IconButton>
        )}
      </Box>

      <Box sx={{ flex: 1, overflow: 'auto' }}>
        {!isInitialized ? (
          <Box sx={{ p: 2, textAlign: 'center' }}>
            <Typography variant="body2" color="text.secondary">
              Carregando permissões...
            </Typography>
          </Box>
        ) : (
          <List sx={{ py: 1 }}>
            {filteredMenuSections.map((section, sectionIndex) => (
              <Box key={section.id}>
                {sectionIndex > 0 && <Divider sx={{ my: 1 }} />}
                <ListItem disablePadding>
                  <ListItemButton
                    onClick={() => handleSectionClick(section)}
                    selected={isActiveSection(section)}
                    sx={{
                      mx: 1,
                      borderRadius: 1,
                      '&.Mui-selected': {
                        backgroundColor: 'primary.main',
                        color: 'primary.contrastText',
                        '&:hover': { backgroundColor: 'primary.dark' },
                        '& .MuiListItemIcon-root': { color: 'primary.contrastText' },
                      },
                    }}
                  >
                    <ListItemIcon sx={{ minWidth: 40 }}>{section.icon}</ListItemIcon>
                    <ListItemText
                      primary={section.title}
                      primaryTypographyProps={{
                        fontSize: '0.875rem',
                        fontWeight: isActiveSection(section) ? 600 : 400,
                      }}
                    />
                    {section.children && (
                      expandedSections.includes(section.id) ? <ExpandLess /> : <ExpandMore />
                    )}
                  </ListItemButton>
                </ListItem>

                {section.children && (
                  <Collapse in={expandedSections.includes(section.id)} timeout="auto" unmountOnExit>
                    <List component="div" disablePadding>
                      {section.children.map((item) => (
                        <ListItem key={item.id} disablePadding>
                          <ListItemButton
                            onClick={() => handleItemClick(item)}
                            selected={isActiveItem(item.path)}
                            disabled={!!item.badge}
                            sx={{
                              pl: 4,
                              mx: 1,
                              borderRadius: 1,
                              '&.Mui-selected': {
                                backgroundColor: 'primary.light',
                                color: 'primary.contrastText',
                              },
                            }}
                          >
                            <ListItemIcon sx={{ minWidth: 36 }}>{item.icon}</ListItemIcon>
                            <ListItemText
                              primary={item.title}
                              primaryTypographyProps={{
                                fontSize: '0.8rem',
                                fontWeight: isActiveItem(item.path) ? 600 : 400,
                              }}
                            />
                            {item.badge && (
                              <Typography
                                variant="caption"
                                sx={{
                                  bgcolor: 'warning.light',
                                  color: 'warning.contrastText',
                                  px: 1,
                                  py: 0.25,
                                  borderRadius: 1,
                                  fontSize: '0.7rem',
                                }}
                              >
                                {item.badge}
                              </Typography>
                            )}
                          </ListItemButton>
                        </ListItem>
                      ))}
                    </List>
                  </Collapse>
                )}
              </Box>
            ))}
          </List>
        )}
      </Box>

      <Box sx={{ p: 2, borderTop: `1px solid ${theme.palette.divider}`, bgcolor: 'grey.50' }}>
        <Typography variant="caption" color="text.secondary" align="center" display="block">
          Versão 1.0.0
        </Typography>
      </Box>
    </Box>
  );

  return (
    <Drawer
      variant={isMobile ? "temporary" : "persistent"}
      open={open}
      onClose={onClose}
      sx={{
        width: open ? layout.sidebarWidth : 0,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: layout.sidebarWidth,
          boxSizing: 'border-box',
          borderRight: '1px solid rgba(0, 0, 0, 0.12)',
        },
      }}
    >
      {drawerContent}
    </Drawer>
  );
};